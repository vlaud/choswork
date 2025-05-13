using System;
using UnityEngine;

[Serializable]
public struct CameraSet
{
    public Transform myRig;
    public Transform DesirePos;
    public GameObject realCam;
    public Vector3 curRot;

    public void SetCamPos()
    {
        realCam.transform.position = DesirePos.position;
        realCam.transform.rotation = DesirePos.rotation;
    }
}

public enum CamState
{
    FPS, UI
}

public enum ViewState
{
    Create, FPS, TPS, UI, Turn,
}

public class CameraProperty : MonoBehaviour
{
    [Header("fps 설정")]
    public Transform myRoot;//fps 좌우값
    public Transform myEyes; //fps카메라 눈에 고정
    [Header("tps 설정")]
    public Transform mySpring; //tps 좌우값
    [Header("ui 설정")]
    public Transform myUI_basePos; //UI카메라 원래위치
    [Header("캐릭터 모델 설정")]
    public Transform myModel; //캐릭터 모델
    public Transform myModel_baseForward; //UI상태일때 캐릭터 모델이 바라봐야 할 곳
    [Header("카메라 설정")]
    public LayerMask crashMask;
    public Vector2 LookupRange = new Vector2(-60.0f, 80.0f);
    public Vector3 curRot = Vector3.zero;
    public Vector3 camPos = Vector3.zero;
    public Vector2 ZoomRange = new Vector2(-3, -0.8f);
    public float LookupSpeed = 10.0f;
    public float ZoomSpeed = 3.0f;
    public float Offset = 0.1f;
    public float desireDistance = 0.0f;
    public float myRotSpeed;
    public bool IsFps = true;
}
