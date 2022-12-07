using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.SceneView;

public class SpringArms : CameraProperty
{
    public enum ViewState
    {
        Create, FPS, TPS, UI
    }
    public ViewState myCameraState = ViewState.Create;
    public CameraSet myFPSCam;
    public CameraSet myTPSCam;
    public CameraSet myUICam;
    public Transform myEyes;
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
                myTPSCam = CopyPaste(myTPSCam, myFPSCam);
                break;
            case ViewState.TPS:
                SelectCamera(myTPSCam);
                break;
            case ViewState.UI:
                SelectCamera(myUICam);
                break;
        }
    }
    void StateProcess()
    {
        if (IsUI) // UI 카메라가 우선시, 켜지면 다른 카메라 비활성화
            ChangeState(ViewState.UI);
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
        //myUICam.myCam.GetComponent<Transform>().position = myTPSCam.myCam.GetComponent<Transform>().position;
        //myUICam.myCam.GetComponent<Transform>().position = myFPSCam.myCam.GetComponent<Transform>().position;
        myTPSCam = SpringArmWork(myTPSCam);
        KeyMovement();
        MouseWheelMove();
        switch (myCameraState)
        {
            case ViewState.Create:
                break;
            case ViewState.FPS:
                myFPSCam = SpringArmWork(myFPSCam);
                break;
            case ViewState.TPS:
                if (Input.GetKey(KeyCode.W))
                {
                    RotatingRoot(mySpring);
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    StartCoroutine(RotatingDownUP());
                }
                
                break;
            case ViewState.UI:
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
    IEnumerator UIRotating(Transform tr)
    {
        //UI카메라 시점 변환, 한번 누르면 됨
        Vector3 dir = tr.forward;
        float Angle = Vector3.Angle(myFPSCam.myRig.forward, dir);
        float rotDir = 1.0f;
        if (Vector3.Dot(myFPSCam.myRig.right, dir) < 0.0f)
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

            myRoot.Rotate(Vector3.up * delta * rotDir, Space.World);
            yield return null;
        }
        myFPSCam = CopyPaste(myFPSCam, myTPSCam);
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
        camPos = myTPSCam.myCam.GetComponent<Transform>().localPosition;

        desireDistance = camPos.z;
        myFPSCam = CameraSetting(myFPSCam);
        myTPSCam = CameraSetting(myTPSCam);
        myUICam = CameraSetting(myUICam);
        AllCameraOff();
        ChangeState(ViewState.FPS);
    }
    // Update is called once per frame
    void Update()
    {
        myFPSCam.myCam.GetComponent<Transform>().position = myEyes.position; // 1인칭 카메라 위치를 캐릭터 눈에 고정
        StateProcess();
    }
    void KeyMovement() // 키보드 조작
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            IsFps = Toggling(IsFps);
            Debug.Log(IsFps);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            IsUI = Toggling(IsUI);
            Debug.Log(IsUI);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
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

    public void MouseWheelMove() // 3인칭 시야 거리
    {
        if (myCameraState == ViewState.TPS)
        {
            desireDistance += Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
            desireDistance = Mathf.Clamp(desireDistance, ZoomRange.x, ZoomRange.y);
        }
        if (Physics.Raycast(myTPSCam.myRig.position, -myTPSCam.myRig.forward, out RaycastHit hit, -camPos.z + Offset, crashMask))
        {
            camPos.z = -hit.distance + Offset;
        }
        else
        {
            camPos.z = Mathf.Lerp(camPos.z, desireDistance, Time.deltaTime * 3.0f);
        }
        myTPSCam.myCam.GetComponent<Transform>().localPosition = camPos;
    }
    void SelectCamera(CameraSet cam) // 카메라 선택
    {
        if (cam.myCam.activeSelf) return;

        AllCameraOff();
        cam.myCam.SetActive(true);
    }
    bool Toggling(bool b) // 토글 기능
    {
        bool bRes = false;

        if (b) bRes = false;
        else bRes = true;

        return bRes;
    }
    void AllCameraOff() // 모든 카메라 끄기
    {
        myFPSCam.myCam.SetActive(false);
        myTPSCam.myCam.SetActive(false);
        myUICam.myCam.SetActive(false);
    }
}
