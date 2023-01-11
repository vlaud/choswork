using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CameraSet
{
    public Transform myRig;
    public GameObject myCam;
    public Vector3 curRot;
}
public class CameraProperty : InputManager
{
    public Transform myRoot;//fps �¿찪
    public Transform mySpring; //tps �¿찪
    public Transform myEyes; //fpsī�޶� ���� ����
    public Transform myUI_basePos; //UIī�޶� ������ġ
    public Transform myModel; //ĳ���� ��
    public Transform myModel_baseForward; //UI�����϶� ĳ���� ���� �ٶ���� �� ��
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
    public bool IsUI = false;
   
}
