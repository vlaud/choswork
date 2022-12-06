using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringArms : MonoBehaviour
{
    public enum ViewState
    {
        Create, FPS, TPS, UI
    }
    public ViewState myCameraState = ViewState.Create;
    public CameraSet myFPSCam;
    public CameraSet myTPSCam;
    public CameraSet myUICam;
    public Vector2 LookupRange = new Vector2(-60.0f, 80.0f);
    public float LookupSpeed = 10.0f;
    public float ZoomSpeed = 3.0f;
    public float Offset = 0.1f;
    public Vector3 curRot = Vector3.zero;
    public Vector3 camPos = Vector3.zero;
    public Vector2 ZoomRange = new Vector2(-4, -0.8f);
    public LayerMask crashMask;
    public bool IsFps = true;
    public bool IsUI = false;
    public float desireDistance = 0.0f;

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
        myFPSCam = SpringArmWork(myFPSCam);
        myTPSCam = SpringArmWork(myTPSCam);
        MouseWheelMove();
        switch (myCameraState)
        {
            case ViewState.Create:
                break;
            case ViewState.FPS:
               
                break;
            case ViewState.TPS:
                break;
            case ViewState.UI:
                break;
        }
    }
    public CameraSet CameraSetting(CameraSet s)
    {
        CameraSet set = s;

        set.curRot.x = set.myRig.localRotation.eulerAngles.x;
        set.curRot.y = set.myRig.parent.localRotation.eulerAngles.y;

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
    public CameraSet SpringArmWork(CameraSet s)
    {
        CameraSet set = s;

        set.curRot.x -= Input.GetAxisRaw("Mouse Y") * LookupSpeed;
        set.curRot.x = Mathf.Clamp(set.curRot.x, LookupRange.x, LookupRange.y);

        set.curRot.y += Input.GetAxisRaw("Mouse X") * LookupSpeed;
        set.myRig.localRotation = Quaternion.Euler(set.curRot.x, 0, 0);
        set.myRig.parent.localRotation = Quaternion.Euler(0, set.curRot.y, 0);

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
    // Update is called once per frame
    void Update()
    {
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
