using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class SpringArms : CameraProperty
{
    public enum ViewState
    {
        Create, FPS, TPS, UI, Turn, Temp
    }
    public ViewState myCameraState = ViewState.Create; // 카메라 상태기계
    public CameraSet myFPSCam;
    public CameraSet myTPSCam;
    public CameraSet myUICam;
    public GameObject GetInventory;
    public Player myPlayer;
    public bool UIkeyAvailable = true;
    public bool isFPSCamRotinTPS = false;

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
                myTPSCam = CopyPaste(myTPSCam, myFPSCam); // 3인칭에서 1인칭 전환시 3인칭 값을 1인칭 값으로
                break;
            case ViewState.TPS:
                SelectCamera(myTPSCam);
                break;
            case ViewState.UI:
                if (IsFps)
                    UICameraSetPos(myFPSCam);
                else
                    UICameraSetPos(myTPSCam);
                StartCoroutine(UIMoving(myUI_basePos));
                StartCoroutine(UIRotating(myModel_baseForward.forward, Space.World)); // UI때 모델 회전
                SelectCamera(myUICam);
                break;
            case ViewState.Turn:
                if (IsFps) // fps 활성화
                {
                    StartCoroutine(UIMoving(myFPSCam.myCam.transform, () => ChangeState(ViewState.FPS)));
                }
                else // tps 활성화
                {
                    StartCoroutine(UIMoving(myTPSCam.myCam.transform, () => ChangeState(ViewState.TPS)));
                }
                StartCoroutine(UIRotating(myRoot.forward, Space.World)); // 모델 회전값을 fps 회전값과 맞춘다
                break;
            case ViewState.Temp:
                break;
        }
    }

    void StateProcess()
    {
        if (myCameraState != ViewState.UI)
            myTPSCam = SpringArmWork(myTPSCam); // 1인칭, 3인칭 카메라값을 같게 

        HandleCameraSwitching(); // 키설정
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
                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                {
                    RotatingRoot(mySpring); // 이동키 꾹 누를시 캐릭터 회전
                }
                if (myPlayer.ReturnAnim().GetBool("IsMoving") && !isFPSCamRotinTPS)
                {
                    StartCoroutine(RotatingDownUP());  //캐릭터가 움직일때 fps 카메라 상하값 정중앙으로
                    isFPSCamRotinTPS = true;
                }
                if (!myPlayer.ReturnAnim().GetBool("IsMoving"))
                    isFPSCamRotinTPS = false; //캐릭터가 안움직이면 고정 해제
                break;
            case ViewState.UI:
                isFPSCamRotinTPS = false;
                break;
            case ViewState.Turn:
                break;
        }
    }
    void CameraCheck()
    {
        if (IsUI) // UI 카메라가 우선시, 켜지면 다른 카메라 비활성화
            ChangeState(ViewState.UI);
        else
        {
            if (myCameraState == ViewState.UI)
                ChangeState(ViewState.Turn);
            else
            {
                if (IsFps) // fps 활성화
                {
                    ChangeState(ViewState.FPS);
                }
                else // tps 활성화
                {
                    ChangeState(ViewState.TPS);
                }
            }
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
        float tpsXr = myTPSCam.myRig.rotation.x; //tps 상하값
        float fpsXr = myFPSCam.myRig.localRotation.eulerAngles.x; //fps 오일러 상하값
        float tpxYr = mySpring.rotation.y; //tps 좌우값
        float fpxYr = myRoot.rotation.y;//fps 좌우값
                                        //mySpring.forward; tps 좌우
                                        //myTPSCam.myRig; tps 상하
                                        //myRoot fps 좌우
                                        //myFPSCam.myRig fps 상하

        float Angle = 0.0f;
        float rotDir = 1.0f;

        //x축 회전이 180이 넘으면 360빼기
        fpsXr = RotationSetTo_180(fpsXr);

        if (fpsXr > 0.0f)
        {
            rotDir = -rotDir;
        }
        Angle = Mathf.Abs(fpsXr);
        while (Angle > 0.0f)
        {
            float delta = myRotSpeed * Time.deltaTime;

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
    IEnumerator UIMoving(Transform tr, UnityAction done = null) //UI카메라 활성화때 시점 자연스럽게 움직임
    {
        Transform cam = myUICam.myCam.transform;
        Vector3 dir = tr.position - cam.position;
        float dist = dir.magnitude;
        dir.Normalize();

        while (dist > 0.0f)
        {
            float delta = LookupSpeed * Time.deltaTime;
            if (delta > dist)
            {
                delta = dist;
            }
            dist -= delta;
            cam.Translate(dir * delta, Space.World);

            yield return null;
        }
        done?.Invoke();
    }
    IEnumerator UIRotating(Vector3 pos, Space sp)
    {
        UIkeyAvailable = false; // 잠깐 i키 안먹히게
        //UI카메라 캐릭터 모델 돌리기
        Vector3 dir = pos;
        float Angle = Vector3.Angle(myModel.forward, dir);

        float rotDir = 1.0f;
        if (Vector3.Dot(myModel.right, dir) < 0.0f)
        {
            rotDir = -rotDir;
        }
        while (Angle > 0.0f)
        {
            float delta = myRotSpeed * Time.deltaTime;

            if (delta > Angle)
            {
                delta = Angle;
            }

            Angle -= delta;

            myModel.Rotate(Vector3.up * delta * rotDir, sp);
            yield return null;
        }
        UIkeyAvailable = true; // i키 다시 활성화
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
            float delta = myRotSpeed * Time.deltaTime;

            if (delta > Angle)
            {
                delta = Angle;
            }

            Angle -= delta;
            myRoot.Rotate(Vector3.up * delta * rotDir, Space.World);
        }
        myFPSCam = CameraSetting(myFPSCam);
        myFPSCam = CopyCurRot(myFPSCam, myFPSCam);
    }
    public CameraSet CameraSetting(CameraSet s)
    {
        CameraSet set = s;
        //x축 회전이 180이 넘으면 360빼기
        set.curRot.x = RotationSetTo_180(set.myRig.localRotation.eulerAngles.x);
        set.curRot.y = set.myRig.parent.localRotation.eulerAngles.y;
        return set;
    }
    public CameraSet CopyCurRot(CameraSet origin, CameraSet copy) // 회전값 복사
    {
        CameraSet set = origin;

        set.curRot.x = copy.curRot.x;
        set.curRot.y = copy.curRot.y;

        return set;
    }
    public CameraSet CopyPaste(CameraSet origin, CameraSet copy) // 오일러를 쿼터니언으로 변환
    {
        CameraSet set = origin;

        set.myRig.localRotation = Quaternion.Euler(copy.curRot.x, 0, 0);
        set.myRig.parent.localRotation = Quaternion.Euler(0, copy.curRot.y, 0);
        set = CopyCurRot(set, copy);

        return set;
    }
    // Start is called before the first frame update
    void Start()
    {
        myInventory = GetInventory;
        camPos = myTPSCam.myCam.transform.localPosition;
        desireDistance = camPos.z;
        myPlayer = transform.parent.GetComponent<Player>();
        myFPSCam = CameraSetting(myFPSCam);
        myTPSCam = CameraSetting(myTPSCam);
        myUICam = CameraSetting(myUICam);
        AllCameraOff();
        ChangeState(ViewState.FPS);
    }
    // Update is called once per frame
    void Update()
    {
        myModel_baseForward.position = myModel.position;

        myFPSCam.myCam.transform.position = myEyes.position; // 1인칭 카메라 위치를 캐릭터 눈에 고정
        myUI_basePos.parent.rotation = mySpring.rotation; // ui카메라(부모를 중점으로)를 3인칭 좌우 회전값과 동일하게
        UICameraSetRot(mySpring);
        StateProcess();
    }
    public override void DebugCamera()
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
    public override void ToggleCam(CamState cam)
    {
        switch(cam)
        {
            case CamState.FPS:
                Toggle.Inst.Toggling(ref IsFps);
                CameraCheck();
                break;
            case CamState.UI:
                Toggle.Inst.Toggling(ref IsUI);
                CameraCheck();
                break;
        }
    }
    public override bool GetIsUI()
    {
        return UIkeyAvailable;
    }
    void UICameraSetPos(CameraSet cam) // UI 카메라 위치 설정
    {
        myUICam.myCam.transform.position = cam.myCam.transform.position;
    }
    void UICameraSetRot(Transform tr) // UI 카메라 회전 설정
    {
        myUICam.myRig.rotation = tr.rotation; //UI 카메라 리그가 돌게끔
    }
    public CameraSet SpringArmWork(CameraSet s) // 카메라 마우스
    {
        CameraSet set = s;
        if (Input.GetMouseButton(1))
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
        Debug.DrawRay(myTPSCam.myCam.transform.position, -myTPSCam.myRig.forward * (Offset), Color.green);
        if (Physics.Raycast(myTPSCam.myRig.position, -myTPSCam.myRig.forward, out RaycastHit hit, -camPos.z + Offset, crashMask))
        {
            camPos.z = -hit.distance + Offset;
        }
        else if (Physics.Raycast(myTPSCam.myCam.transform.position, -myTPSCam.myRig.forward, out RaycastHit thit, 0.01f + Offset, crashMask))
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                camPos.z = Mathf.Lerp(camPos.z, desireDistance, Time.deltaTime * ZoomSpeed);
            }
        }
        else
        {
            camPos.z = Mathf.Lerp(camPos.z, desireDistance, Time.deltaTime * ZoomSpeed);
        }
        myTPSCam.myCam.transform.localPosition = camPos;
    }
    void SelectCamera(CameraSet cam) // 카메라 선택
    {
        if (cam.myCam.activeSelf) return;

        AllCameraOff();
        cam.myCam.SetActive(true);
    }
    void CamerasOnOff(ViewState s)
    {
        bool bFps = false;
        bool bTps = false;
        bool bUI = false;
        switch (s)
        {
            case ViewState.FPS:
                bFps = true;
                break;
            case ViewState.TPS:
                bTps = true;
                break;
            case ViewState.UI:
                bUI = true;
                break;
        }
        myFPSCam.myCam.SetActive(bFps);
        myTPSCam.myCam.SetActive(bTps);
        myUICam.myCam.SetActive(bUI);
    }
    void AllCameraOff() // 모든 카메라 끄기
    {
        myFPSCam.myCam.SetActive(false);
        myTPSCam.myCam.SetActive(false);
        myUICam.myCam.SetActive(false);
    }
}
