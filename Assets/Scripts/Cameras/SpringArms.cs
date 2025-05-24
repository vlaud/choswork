using Commands.Camera;
using Project.Tools.InterfaceHelp;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SpringArms : CameraProperty, EventListener<CameraStatesEvent>, iSubscription, iSpringArmFunctionality
{
    [Header("카메라 상태 설정")]
    public ViewState myCameraState = ViewState.Create; // 카메라 상태기계
    public CameraSet myFPSCam;
    public CameraSet myTPSCam;
    public CameraSet myUICam;
    [SerializeField] private InterfaceHolder<iPlayerFunctionality> myPlayer;
    public bool isFPSCamRotinTPS = false;
    private bool isGhost = false;

    private Coroutine currentUIRotatingCoroutine; // 현재 실행 중인 UI 회전 코루틴
    private Coroutine currentUIMovingCoroutine; // 현재 실행 중인 UI 움직임 코루틴


    private iCameraContextProvider _contextProvider;
    private bool IsFps
    {
        get
        {
            if (_contextProvider?.Context == null)
            {
                Debug.LogWarning("Camera context not ready");
                return false;
            }
            return _contextProvider.Context.Mode == CameraMode.FPS;
        }
    }
    public GameObject[] GetAllCams()
    {
        GameObject[] cams = new GameObject[3];

        if (myFPSCam.realCam != null) cams[0] = myFPSCam.realCam;
        if (myTPSCam.realCam != null) cams[1] = myTPSCam.realCam;
        if (myUICam.realCam != null) cams[2] = myUICam.realCam;

        return cams;
    }

    void ChangeState(ViewState s)
    {
        if (myCameraState == s) return;
        myCameraState = s;

        switch (myCameraState)
        {
            case ViewState.Create:
                break;
            case ViewState.FPS:
                SelectCamera(myFPSCam);
                CopyPaste(ref myTPSCam, myFPSCam); // 3인칭에서 1인칭 전환시 3인칭 값을 1인칭 값으로
                break;
            case ViewState.TPS:
                SelectCamera(myTPSCam);
                break;
            case ViewState.UI:
                UICameraSetPos(IsFps ? myFPSCam : myTPSCam);

                this.StartOrRestartCoroutine(ref currentUIMovingCoroutine, UIMoving(myUI_basePos));
                this.StartOrRestartCoroutine(ref currentUIRotatingCoroutine, UIRotating(myModel_baseForward.forward, true));

                SelectCamera(myUICam);
                break;
            case ViewState.Turn:
                ViewState viewState = IsFps ? ViewState.FPS : ViewState.TPS;
                CameraSet cameraSet = IsFps ? myFPSCam : myTPSCam;
                this.StartOrRestartCoroutine(ref currentUIMovingCoroutine, UIMoving(cameraSet.DesirePos, () => ChangeState(viewState)));

                this.StartOrRestartCoroutine(ref currentUIRotatingCoroutine, UIRotating(myRoot.forward, false));
                break;
        }
    }

    void StateProcess()
    {
        if (GameManagement.Inst.myGameState != GameState.Play) return;

        if (myCameraState != ViewState.UI)
            myTPSCam = SpringArmWork(myTPSCam); // 1인칭, 3인칭 카메라값을 같게 

        MouseWheelMove(); // 3인칭 시야 거리
        switch (myCameraState)
        {
            case ViewState.Create:
                break;
            case ViewState.FPS:
                isFPSCamRotinTPS = false;
                myFPSCam = SpringArmWork(myFPSCam);
                break;
            case ViewState.TPS: // 3인칭
                if (myPlayer.Value.GetAnimator().GetBool("IsMoving"))
                {
                    RotatingRoot(mySpring); // 이동키 꾹 누를시 캐릭터 회전

                    if (!isFPSCamRotinTPS)
                    {
                        StartCoroutine(RotatingDownUP());  //캐릭터가 움직일때 fps 카메라 상하값 정중앙으로
                        isFPSCamRotinTPS = true;
                    }
                }
                if (!myPlayer.Value.GetAnimator().GetBool("IsMoving"))
                    isFPSCamRotinTPS = false; //캐릭터가 안움직이면 고정 해제
                break;
            case ViewState.UI:
                isFPSCamRotinTPS = false;
                break;
            case ViewState.Turn:
                break;
        }
    }

    float RotationSetTo_180(float angle)
    {
        // -180 ~ 180으로 고정
        if (angle > 180.0f)
        {
            angle = angle - 360.0f;
        }
        else if (angle < -180.0f)
        {
            angle = angle + 360.0f;
        }

        return angle;
    }

    IEnumerator RotatingDownUP()
    {
        //fps카메라 위아래 바꾸기
        float fpsXr = myFPSCam.myRig.localRotation.eulerAngles.x; //fps 오일러 상하값
        float rotDir = 1.0f;

        //x축 회전이 180이 넘으면 360빼기
        fpsXr = RotationSetTo_180(fpsXr);

        if (fpsXr > Mathf.Epsilon)
        {
            rotDir = -rotDir;
        }

        float Angle = Mathf.Abs(fpsXr);

        while (Angle > Mathf.Epsilon)
        {
            float delta = myRotSpeed * Time.unscaledDeltaTime;

            if (delta > Angle)
            {
                delta = Angle;
            }

            Angle -= delta;

            myFPSCam.myRig.Rotate(Vector3.right * delta * rotDir);
            yield return null;
        }
        myFPSCam = CameraSetting(myFPSCam);
        CopyCurRot(ref myFPSCam, myFPSCam);
    }
    
    IEnumerator UIMoving(Transform tr, UnityAction done = null) //UI카메라 활성화때 시점 자연스럽게 움직임
    {
        Vector3 dir = tr.position - myUICam.DesirePos.position;
        float dist = dir.magnitude;
        dir.Normalize();

        while (dist > Mathf.Epsilon)
        {
            dir = tr.position - myUICam.DesirePos.position;
            dist = dir.magnitude;
            dir.Normalize();
            float delta = LookupSpeed * Time.unscaledDeltaTime;
            if (delta > dist)
            {
                delta = dist;
            }
            dist -= delta;
            myUICam.DesirePos.Translate(dir * delta, Space.World);

            yield return null;
        }
        done?.Invoke();
    }

    IEnumerator UIRotating(Vector3 dir, bool IsUI, Space sp = Space.World)
    {
        //UI카메라 캐릭터 모델 돌리기
        float Angle = Vector3.Angle(myModel.forward, dir);
        float rotDir = 1.0f;
        if (Vector3.Dot(myModel.right, dir) < Mathf.Epsilon)
        {
            rotDir = -rotDir;
        }
        while (Angle > Mathf.Epsilon)
        {
            if (IsUI) dir = myModel_baseForward.forward;
            else dir = myRoot.forward;
            Angle = Vector3.Angle(myModel.forward, dir);

            float delta = myRotSpeed * Time.unscaledDeltaTime;

            if (delta > Angle)
            {
                delta = Angle;
            }

            Angle -= delta;

            myModel.Rotate(Vector3.up * delta * rotDir, sp);
            yield return null;
        }
    }
    void RotatingRoot(Transform tr)
    {
        // 캐릭터 3인칭 시점 변환, 눌렀다 떼면 회전 중지
        Vector3 dir = tr.forward;
        float Angle = Vector3.Angle(myRoot.forward, dir);
        float rotDir = 1.0f;

        if (Vector3.Dot(myRoot.right, dir) < 0.0f)
        {
            rotDir = -rotDir;
        }
        if (Angle > 0.0f)
        {
            float delta = myRotSpeed * Time.unscaledDeltaTime;

            if (delta > Angle)
            {
                delta = Angle;
            }

            myRoot.Rotate(Vector3.up * delta * rotDir, Space.World);
        }
        myFPSCam = CameraSetting(myFPSCam);
        CopyCurRot(ref myFPSCam, myFPSCam);
    }

    public CameraSet CameraSetting(CameraSet s)
    {
        CameraSet set = s;
        //x축 회전이 180이 넘으면 360빼기
        set.curRot.x = RotationSetTo_180(set.myRig.localRotation.eulerAngles.x);
        set.curRot.y = set.myRig.parent.localRotation.eulerAngles.y;
        return set;
    }

    public void CopyCurRot(ref CameraSet origin, CameraSet copy) // 회전값 복사
    {
        origin.curRot.x = copy.curRot.x;
        origin.curRot.y = copy.curRot.y;
    }

    public void CopyPaste(ref CameraSet origin, CameraSet copy) // 오일러를 쿼터니언으로 변환
    {
        origin.myRig.localRotation = Quaternion.Euler(copy.curRot.x, 0, 0);
        origin.myRig.parent.localRotation = Quaternion.Euler(0, copy.curRot.y, 0);
        CopyCurRot(ref origin, copy);
    }

    private void Awake()
    {
        if (!isGhost)
        {
            SetPlayer(GameManagement.Inst.myPlayer);
            CameraActions.SetKeys(this);
            _contextProvider = CameraActions.Inst;
        }

        // IPlayerFunctionality를 상속받는 클래스들을 ComponentTypeFinder를 통해 찾아내어 InterfaceHolder에 저장
        myPlayer.SetValue(ComponentTypeFinder.FindFirstImplementing<iPlayerFunctionality>());
    }

    // Start is called before the first frame update
    void Start()
    {
        camPos = myTPSCam.DesirePos.localPosition;
        desireDistance = camPos.z;

        myFPSCam = CameraSetting(myFPSCam);
        myTPSCam = CameraSetting(myTPSCam);
        myUICam = CameraSetting(myUICam);
        AllCameraOff();
        Subscribe();
        ChangeState(ViewState.FPS);
    }

    // Update is called once per frame
    void Update()
    {
        myFPSCam.SetCamPos();
        myTPSCam.SetCamPos();
        myUICam.SetCamPos();

        myModel_baseForward.position = myModel.position;

        myFPSCam.DesirePos.position = myEyes.position; // 1인칭 카메라 위치를 캐릭터 눈에 고정
        myUI_basePos.parent.rotation = mySpring.rotation; // ui카메라(부모를 중점으로)를 3인칭 좌우 회전값과 동일하게
        UICameraSetRot(mySpring);
        StateProcess();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void DebugCamera()
    {
        Debug.Log(myModel.localRotation.eulerAngles.y);
        Debug.Log("쿼터니언 : " + myFPSCam.myRig.rotation.x);
        Debug.Log("오일러 : " + myFPSCam.myRig.rotation.eulerAngles.x);
        if (myFPSCam.myRig.localRotation.eulerAngles.x > 180.0f)
        {
            Debug.Log("로컬오일러 : " + (myFPSCam.myRig.localRotation.eulerAngles.x - 360.0f));
        }
        else if (myFPSCam.myRig.localRotation.eulerAngles.x < -180.0f)
        {
            Debug.Log("로컬오일러 : " + (myFPSCam.myRig.localRotation.eulerAngles.x + 360.0f));
        }
    }

    void UICameraSetPos(CameraSet cam) // UI 카메라 위치 설정
    {
        myUICam.DesirePos.position = cam.DesirePos.position;
    }

    void UICameraSetRot(Transform tr) // UI 카메라 회전 설정
    {
        myUICam.myRig.rotation = tr.rotation; //UI 카메라 리그가 돌게끔
    }

    public CameraSet SpringArmWork(CameraSet s) // 카메라 마우스
    {
        CameraSet set = s;
        if (CursorManager.Instance.IsCurSorLocked())
        {
            set.curRot.x -= Input.GetAxisRaw("Mouse Y") * LookupSpeed;
            set.curRot.x = Mathf.Clamp(set.curRot.x, LookupRange.x, LookupRange.y);

            set.curRot.y += Input.GetAxisRaw("Mouse X") * LookupSpeed;
            set.myRig.localRotation = Quaternion.Euler(set.curRot.x, 0, 0);
            set.myRig.parent.localRotation = Quaternion.Euler(0, set.curRot.y, 0);
        }
        return set;
    }

    public CameraSet? GetMyCamera() //현재 카메라 트랜스폼 리턴
    {
        switch (myCameraState)
        {
            case ViewState.FPS:
                return myFPSCam;
            case ViewState.TPS:
                return myTPSCam;
        }
        return null;
    }

    public void MouseWheelMove() // 3인칭 시야 거리
    {
        if (myCameraState == ViewState.TPS)
        {
            desireDistance += Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
            desireDistance = Mathf.Clamp(desireDistance, ZoomRange.x, ZoomRange.y);
        }
        Debug.DrawRay(myTPSCam.myRig.position, -myTPSCam.myRig.forward * (-camPos.z + Offset), Color.red);
        Debug.DrawRay(myTPSCam.DesirePos.position, -myTPSCam.myRig.forward * (Offset), Color.green);
        if (Physics.Raycast(myTPSCam.myRig.position, -myTPSCam.myRig.forward, out RaycastHit hit, -camPos.z + Offset, crashMask))
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                desireDistance = Mathf.Clamp(desireDistance, camPos.z, ZoomRange.y);
                camPos.z = Mathf.Lerp(camPos.z, desireDistance, Time.unscaledDeltaTime * ZoomSpeed);
            }
            else
            {
                camPos.z = -hit.distance + Offset;
            }

        }
        else if (Physics.Raycast(myTPSCam.DesirePos.position, -myTPSCam.myRig.forward, 0.01f + Offset, crashMask))
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                desireDistance = Mathf.Clamp(desireDistance, camPos.z, ZoomRange.y);
                camPos.z = Mathf.Lerp(camPos.z, desireDistance, Time.unscaledDeltaTime * ZoomSpeed);
            }
        }
        else
        {
            camPos.z = Mathf.Lerp(camPos.z, desireDistance, Time.unscaledDeltaTime * ZoomSpeed);
        }
        myTPSCam.DesirePos.localPosition = camPos;
    }

    void SelectCamera(CameraSet cam) // 카메라 선택
    {
        if (cam.realCam.activeSelf) return;

        AllCameraOff();
        cam.realCam.SetActive(true);
    }

    void AllCameraOff() // 모든 카메라 끄기
    {
        myFPSCam.realCam.SetActive(false);
        myTPSCam.realCam.SetActive(false);
        myUICam.realCam.SetActive(false);
    }

    public void GhostSet(bool vGhost = false)
    {
        isGhost = vGhost;
    }

    public void SetPlayer(iPlayerFunctionality player)
    {
        myPlayer.SetValue(player);
    }

    public void OnEvent(CameraStatesEvent eventType)
    {
        switch (eventType.cameraEventType)
        {
            case CameraEventType.FPS:
                ChangeState(ViewState.FPS);
                break;
            case CameraEventType.TPS:
                ChangeState(ViewState.TPS);
                break;
            case CameraEventType.NotUI:
                ChangeState(ViewState.Turn);
                break;
            case CameraEventType.UI:
                ChangeState(ViewState.UI);
                break;
            case CameraEventType.Debug:
                DebugCamera();
                break;
        }
    }

    public void Subscribe()
    {
        this.EventStartingListening<CameraStatesEvent>();
    }

    public void Unsubscribe()
    {
        this.EventStopListening<CameraStatesEvent>();
    }
}
