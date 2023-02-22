using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneCamera : MonoBehaviour
{
    public CameraSet CutsceneCam;
    public Transform myEyes; //fps카메라 눈에 고정

    public enum State
    {
        Create, Car, Outting, Out
    }
    public State myState = State.Create;

    void ChangeState(State s)
    {
        if (myState == s) return;
        myState = s;

        switch(myState)
        {
            case State.Car:
                break;
            case State.Outting:
                break;
            case State.Out:
                break;
        }
    }
    void StateProcess()
    {
        switch (myState)
        {
            case State.Car:
                CamMovement();
                break;
            case State.Outting:
                CamMovement();
                break;
            case State.Out:
                break;
        }
    }
    void Start()
    {
        ChangeState(State.Car);
    }

    // Update is called once per frame
    void Update()
    {
        StateProcess();
    }
    void CamMovement()
    {
        CutsceneCam.myCam.transform.position = myEyes.position; // 1인칭 카메라 위치를 캐릭터 눈에 고정
        CutsceneCam.curRot.y = myEyes.rotation.eulerAngles.y;
        CutsceneCam.myRig.rotation = Quaternion.Euler(CutsceneCam.curRot);
    }
}
