using Commands.Camera;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SpringArms : CameraProperty, EventListener<CameraStatesEvent>, iSubscription
{
    [Header("ī�޶� ���� ����")]
    public ViewState myCameraState = ViewState.Create; // ī�޶� ���±��
    public CameraSet myFPSCam;
    public CameraSet myTPSCam;
    public CameraSet myUICam;
    public Player myPlayer;
    public bool UIkeyAvailable = true;
    public bool isFPSCamRotinTPS = false;
    private bool isGhost = false;

    private CoroutineRunner coroutineRunner;
    private Coroutine currentUIRotatingCoroutine; // ���� ���� ���� UI ȸ�� �ڷ�ƾ
    private Coroutine currentUIMovingCoroutine; // ���� ���� ���� UI ������ �ڷ�ƾ

    // �ٸ� Ŭ�������� Inventory �ν��Ͻ��� ����
    [SerializeField] Inventory _inventory;

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
                myTPSCam = CopyPaste(myTPSCam, myFPSCam); // 3��Ī���� 1��Ī ��ȯ�� 3��Ī ���� 1��Ī ������
                break;
            case ViewState.TPS:
                SelectCamera(myTPSCam);
                break;
            case ViewState.UI:
                UICameraSetPos(IsFps ? myFPSCam : myTPSCam);

                coroutineRunner.StartCurrentCoroutine(
                    currentUIMovingCoroutine,
                    out currentUIMovingCoroutine,
                    UIMoving(myUI_basePos));

                coroutineRunner.StartCurrentCoroutine(
                    currentUIRotatingCoroutine, 
                    out currentUIRotatingCoroutine, 
                    UIRotating(myModel_baseForward.forward, true));

                SelectCamera(myUICam);
                break;
            case ViewState.Turn:
                ViewState viewState = IsFps ? ViewState.FPS : ViewState.TPS;
                CameraSet cameraSet = IsFps ? myFPSCam : myTPSCam;
                coroutineRunner.StartCurrentCoroutine(
                currentUIMovingCoroutine,
                out currentUIMovingCoroutine,
                UIMoving(cameraSet.DesirePos, () => ChangeState(viewState)));

                coroutineRunner.StartCurrentCoroutine(
                   currentUIRotatingCoroutine,
                   out currentUIRotatingCoroutine,
                   UIRotating(myRoot.forward, false));
                break;
        }
    }

    void StateProcess()
    {
        if (GameManagement.Inst.myGameState != GameState.Play) return;

        if (myCameraState != ViewState.UI)
            myTPSCam = SpringArmWork(myTPSCam); // 1��Ī, 3��Ī ī�޶��� ���� 

        MouseWheelMove(); // 3��Ī �þ� �Ÿ�
        switch (myCameraState)
        {
            case ViewState.Create:
                break;
            case ViewState.FPS:
                isFPSCamRotinTPS = false;
                myFPSCam = SpringArmWork(myFPSCam);
                break;
            case ViewState.TPS: // 3��Ī
                if (myPlayer.ReturnAnim().GetBool("IsMoving"))
                {
                    RotatingRoot(mySpring); // �̵�Ű �� ������ ĳ���� ȸ��

                    if (!isFPSCamRotinTPS)
                    {
                        StartCoroutine(RotatingDownUP());  //ĳ���Ͱ� �����϶� fps ī�޶� ���ϰ� ���߾�����
                        isFPSCamRotinTPS = true;
                    }
                }
                if (!myPlayer.ReturnAnim().GetBool("IsMoving"))
                    isFPSCamRotinTPS = false; //ĳ���Ͱ� �ȿ����̸� ���� ����
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
        // -180 ~ 180���� ����
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
        //fpsī�޶� ���Ʒ� �ٲٱ�
        float fpsXr = myFPSCam.myRig.localRotation.eulerAngles.x; //fps ���Ϸ� ���ϰ�
        float rotDir = 1.0f;

        //x�� ȸ���� 180�� ������ 360����
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
        myFPSCam = CopyCurRot(myFPSCam, myFPSCam);
    }
    IEnumerator UIMoving(Transform tr, UnityAction done = null) //UIī�޶� Ȱ��ȭ�� ���� �ڿ������� ������
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
        //UIkeyAvailable = false; // ��� iŰ �ȸ�����

        //UIī�޶� ĳ���� �� ������
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
        //UIkeyAvailable = true; // iŰ �ٽ� Ȱ��ȭ
    }
    void RotatingRoot(Transform tr)
    {
        // ĳ���� 3��Ī ���� ��ȯ, ������ ���� ȸ�� ����
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
        myFPSCam = CopyCurRot(myFPSCam, myFPSCam);
    }
    public CameraSet CameraSetting(CameraSet s)
    {
        CameraSet set = s;
        //x�� ȸ���� 180�� ������ 360����
        set.curRot.x = RotationSetTo_180(set.myRig.localRotation.eulerAngles.x);
        set.curRot.y = set.myRig.parent.localRotation.eulerAngles.y;
        return set;
    }
    public CameraSet CopyCurRot(CameraSet origin, CameraSet copy) // ȸ���� ����
    {
        CameraSet set = origin;

        set.curRot.x = copy.curRot.x;
        set.curRot.y = copy.curRot.y;

        return set;
    }

    public CameraSet CopyPaste(CameraSet origin, CameraSet copy) // ���Ϸ��� ���ʹϾ����� ��ȯ
    {
        CameraSet set = origin;

        set.myRig.localRotation = Quaternion.Euler(copy.curRot.x, 0, 0);
        set.myRig.parent.localRotation = Quaternion.Euler(0, copy.curRot.y, 0);
        set = CopyCurRot(set, copy);

        return set;
    }

    private void Awake()
    {
        if (!isGhost)
            _inventory = FindFirstObjectByType<Inventory>();


        if (_inventory != null)
        {
            // OnInventoryToggled �̺�Ʈ�� �̺�Ʈ �ڵ鷯 �߰�
            _inventory.OnInventoryToggled += HandleInventoryToggled;

            // OnInventoryToggled �̺�Ʈ���� �̺�Ʈ �ڵ鷯 ����
            // inventory.OnInventoryToggled -= HandleInventoryToggled;
        }

        coroutineRunner = new CoroutineRunner(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        camPos = myTPSCam.DesirePos.localPosition;
        desireDistance = camPos.z;

        if (!isGhost)
        {
            SetPlayer(GameManagement.Inst.myPlayer);
            CameraActions.SetKeys();
        }

        myFPSCam = CameraSetting(myFPSCam);
        myTPSCam = CameraSetting(myTPSCam);
        myUICam = CameraSetting(myUICam);
        AllCameraOff();
        Subscribe();
        ChangeState(ViewState.FPS);
        if (isGhost) _inventory = null;
    }

    // Update is called once per frame
    void Update()
    {
        myFPSCam.SetCamPos();
        myTPSCam.SetCamPos();
        myUICam.SetCamPos();

        myModel_baseForward.position = myModel.position;

        myFPSCam.DesirePos.position = myEyes.position; // 1��Ī ī�޶� ��ġ�� ĳ���� ���� ����
        myUI_basePos.parent.rotation = mySpring.rotation; // uiī�޶�(�θ� ��������)�� 3��Ī �¿� ȸ������ �����ϰ�
        UICameraSetRot(mySpring);
        StateProcess();

        IsFps = CameraActions.GetState() == CameraKeyType.FPS;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void DebugCamera()
    {
        Debug.Log(myModel.localRotation.eulerAngles.y);
        Debug.Log("���ʹϾ� : " + myFPSCam.myRig.rotation.x);
        Debug.Log("���Ϸ� : " + myFPSCam.myRig.rotation.eulerAngles.x);
        if (myFPSCam.myRig.localRotation.eulerAngles.x > 180.0f)
        {
            Debug.Log("���ÿ��Ϸ� : " + (myFPSCam.myRig.localRotation.eulerAngles.x - 360.0f));
        }
        else if (myFPSCam.myRig.localRotation.eulerAngles.x < -180.0f)
        {
            Debug.Log("���ÿ��Ϸ� : " + (myFPSCam.myRig.localRotation.eulerAngles.x + 360.0f));
        }
    }

    void UICameraSetPos(CameraSet cam) // UI ī�޶� ��ġ ����
    {
        myUICam.DesirePos.position = cam.DesirePos.position;
    }

    void UICameraSetRot(Transform tr) // UI ī�޶� ȸ�� ����
    {
        myUICam.myRig.rotation = tr.rotation; //UI ī�޶� ���װ� ���Բ�
    }

    public CameraSet SpringArmWork(CameraSet s) // ī�޶� ���콺
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

    public CameraSet? GetMyCamera() //���� ī�޶� Ʈ������ ����
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

    public void MouseWheelMove() // 3��Ī �þ� �Ÿ�
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

    void SelectCamera(CameraSet cam) // ī�޶� ����
    {
        if (cam.realCam.activeSelf) return;

        AllCameraOff();
        cam.realCam.SetActive(true);
    }

    void AllCameraOff() // ��� ī�޶� ����
    {
        myFPSCam.realCam.SetActive(false);
        myTPSCam.realCam.SetActive(false);
        myUICam.realCam.SetActive(false);
    }

    public void GhostSet(bool vGhost = false)
    {
        isGhost = vGhost;
    }

    public void SetPlayer(Player player)
    {
        myPlayer = player;
    }

    private void HandleInventoryToggled(bool isInventoryOpen)
    {
        if (!UIkeyAvailable) return;
        // �κ��丮�� �����ִ��� ���ο� ���� ó��
        if (isInventoryOpen)
        {
            // �κ��丮�� ���� ��� ó���� ����
            ChangeState(ViewState.UI);
        }
        else
        {
            // �κ��丮�� ���� ��� ó���� ����
            ChangeState(ViewState.Turn);
        }
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
