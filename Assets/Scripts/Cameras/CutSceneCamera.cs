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

        switch (myState)
        {
            case State.Car:
                CutsceneCam.DesirePos.SetParent(myEyes);
                CutsceneCam.DesirePos.localPosition = Vector3.zero;
                break;
            case State.Outting:
                CutsceneCam.DesirePos.SetParent(CutsceneCam.myRig);
                CutsceneCam.DesirePos.localPosition = Vector3.zero;
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
                break;
            case State.Outting:
                CarOutMovement();
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
        CutsceneCam.SetCamPos();
    }

    void CarOutMovement()
    {
        CutsceneCam.DesirePos.position = myEyes.position;
        CutsceneCam.curRot.y = myEyes.rotation.eulerAngles.y;
        CutsceneCam.myRig.rotation = Quaternion.Euler(CutsceneCam.curRot);
    }

    public void CarOut()
    {
        ChangeState(State.Outting);
    }
}
