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
    public ViewState myCameraState = ViewState.Create; // ī�޶� ���±��
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
                myTPSCam = CopyPaste(myTPSCam, myFPSCam); // 3��Ī���� 1��Ī ��ȯ�� 3��Ī ���� 1��Ī ������
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
                StartCoroutine(UIRotating(myModel_baseForward.forward, Space.World)); // UI�� �� ȸ��
                SelectCamera(myUICam);
                break;
            case ViewState.Turn:
                if (IsFps) // fps Ȱ��ȭ
                {
                    StartCoroutine(UIMoving(myFPSCam.myCam.transform, () => ChangeState(ViewState.FPS)));
                }
                else // tps Ȱ��ȭ
                {
                    StartCoroutine(UIMoving(myTPSCam.myCam.transform, () => ChangeState(ViewState.TPS)));
                }
                StartCoroutine(UIRotating(myRoot.forward, Space.World)); // �� ȸ������ fps ȸ������ �����
                break;
            case ViewState.Temp:
                break;
        }
    }

    void StateProcess()
    {
        if (myCameraState != ViewState.UI)
            myTPSCam = SpringArmWork(myTPSCam); // 1��Ī, 3��Ī ī�޶��� ���� 

        HandleCameraSwitching(); // Ű����
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
                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                {
                    RotatingRoot(mySpring); // �̵�Ű �� ������ ĳ���� ȸ��
                }
                if (myPlayer.ReturnAnim().GetBool("IsMoving") && !isFPSCamRotinTPS)
                {
                    StartCoroutine(RotatingDownUP());  //ĳ���Ͱ� �����϶� fps ī�޶� ���ϰ� ���߾�����
                    isFPSCamRotinTPS = true;
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
    void CameraCheck()
    {
        if (IsUI) // UI ī�޶� �켱��, ������ �ٸ� ī�޶� ��Ȱ��ȭ
            ChangeState(ViewState.UI);
        else
        {
            if (myCameraState == ViewState.UI)
                ChangeState(ViewState.Turn);
            else
            {
                if (IsFps) // fps Ȱ��ȭ
                {
                    ChangeState(ViewState.FPS);
                }
                else // tps Ȱ��ȭ
                {
                    ChangeState(ViewState.TPS);
                }
            }
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
        float tpsXr = myTPSCam.myRig.rotation.x; //tps ���ϰ�
        float fpsXr = myFPSCam.myRig.localRotation.eulerAngles.x; //fps ���Ϸ� ���ϰ�
        float tpxYr = mySpring.rotation.y; //tps �¿찪
        float fpxYr = myRoot.rotation.y;//fps �¿찪
                                        //mySpring.forward; tps �¿�
                                        //myTPSCam.myRig; tps ����
                                        //myRoot fps �¿�
                                        //myFPSCam.myRig fps ����

        float Angle = 0.0f;
        float rotDir = 1.0f;

        //x�� ȸ���� 180�� ������ 360����
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
    IEnumerator UIMoving(Transform tr, UnityAction done = null) //UIī�޶� Ȱ��ȭ�� ���� �ڿ������� ������
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
        UIkeyAvailable = false; // ��� iŰ �ȸ�����
        //UIī�޶� ĳ���� �� ������
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
        UIkeyAvailable = true; // iŰ �ٽ� Ȱ��ȭ
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

        myFPSCam.myCam.transform.position = myEyes.position; // 1��Ī ī�޶� ��ġ�� ĳ���� ���� ����
        myUI_basePos.parent.rotation = mySpring.rotation; // uiī�޶�(�θ� ��������)�� 3��Ī �¿� ȸ������ �����ϰ�
        UICameraSetRot(mySpring);
        StateProcess();
    }
    public override void DebugCamera()
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
    void UICameraSetPos(CameraSet cam) // UI ī�޶� ��ġ ����
    {
        myUICam.myCam.transform.position = cam.myCam.transform.position;
    }
    void UICameraSetRot(Transform tr) // UI ī�޶� ȸ�� ����
    {
        myUICam.myRig.rotation = tr.rotation; //UI ī�޶� ���װ� ���Բ�
    }
    public CameraSet SpringArmWork(CameraSet s) // ī�޶� ���콺
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
    void SelectCamera(CameraSet cam) // ī�޶� ����
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
    void AllCameraOff() // ��� ī�޶� ����
    {
        myFPSCam.myCam.SetActive(false);
        myTPSCam.myCam.SetActive(false);
        myUICam.myCam.SetActive(false);
    }
}
