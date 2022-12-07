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

        myTPSCam = SpringArmWork(myTPSCam);
        MouseWheelMove();
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("쿼터니언 : " + myFPSCam.myRig.rotation.x);
            Debug.Log("오일러 : " + myFPSCam.myRig.rotation.eulerAngles.x);
            Debug.Log("로컬오일러 : " + myFPSCam.myRig.localRotation.eulerAngles.x);
        }
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
                if (Input.GetKey(KeyCode.D))
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
    IEnumerator RotatingDownUP()
    {
        //fps카메라 위아래 바꾸기
        float tpsXr = myTPSCam.myRig.rotation.x; //tps 상하값
        float fpsXr = myFPSCam.myRig.rotation.x; //fps 상하값
        float tpxYr = mySpring.rotation.y; //tps 좌우값
        float fpxYr = myRoot.rotation.y;//fps 좌우값
        //mySpring.forward; tps 좌우
        //myTPSCam.myRig; tps 상하
        //myRoot fps 좌우
        //myFPSCam.myRig fps 상하
        Vector3 dir = myFPSCam.myRig.forward;
        dir.x = 0.0f;
        dir.Normalize();
        float Angle = Mathf.Abs(fpsXr);
        float rotDir = 1.0f;

        if (Vector3.Dot(myRoot.up, dir) > 0.0f)
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
        //myFPSCam = CopyPaste(myFPSCam, myTPSCam);
    }
    public CameraSet CameraSetting(CameraSet s)
    {
        CameraSet set = s;

        set.curRot.x = set.myRig.localRotation.eulerAngles.x;
        set.curRot.y = set.myRig.parent.localRotation.eulerAngles.y;
        //x축 회전이 180이 넘으면 360빼기
        if(set.myRig.localRotation.eulerAngles.x > 180.0f)
        {
            set.curRot.x = set.myRig.localRotation.eulerAngles.x - 360.0f;
        }
        return set;
    }
    public CameraSet CopyCurRot(CameraSet origin, CameraSet copy)
    {
        CameraSet set = origin;

        set.curRot.x = copy.curRot.x;
        set.curRot.y = copy.curRot.y;

        return set;
    }
    public CameraSet CopyPaste(CameraSet origin, CameraSet copy)
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
        myFPSCam.myCam.GetComponent<Transform>().position = myEyes.position;
        StateProcess();
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
    }
    public CameraSet SpringArmWork(CameraSet s)
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

    public void MouseWheelMove()
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
    void SelectCamera(CameraSet cam)
    {
        if (cam.myCam.activeSelf) return;

        AllCameraOff();
        cam.myCam.SetActive(true);
    }
    bool Toggling(bool b)
    {
        bool bRes = false;

        if (b) bRes = false;
        else bRes = true;

        return bRes;
    }
    void AllCameraOff()
    {
        myFPSCam.myCam.SetActive(false);
        myTPSCam.myCam.SetActive(false);
        myUICam.myCam.SetActive(false);
    }
}
