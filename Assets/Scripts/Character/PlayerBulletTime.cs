using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBulletTime : MonoBehaviour, iSubscription, EventListener<GameStatesEvent>
{
    public enum State
    {
        Create, Start, Play, Pause
    }
    public State myState = State.Create;

    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    [SerializeField] private Transform _map;
    [SerializeField] private Player _player;
    private Dictionary<AIPerception, AIPerception> _spawnedMonsters = new Dictionary<AIPerception, AIPerception>();
    private Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();
    private Dictionary<ObjectNotGrabbable, ObjectNotGrabbable> _ghostInterables = new Dictionary<ObjectNotGrabbable, ObjectNotGrabbable>();
    [SerializeField] private Transform ghostPlayer;
    [SerializeField] private SpringArms ghostCamera;
    private float time_start;
    [SerializeField] private float time_current;
    private float time_Max = 1f;

    void ChangeState(State s)
    {
        if (myState == s) return;
        myState = s;

        switch (myState)
        {
            case State.Start:
                ChangeAnimUpdateMode(AnimatorUpdateMode.Normal);
                break;
            case State.Play:
                ChangeAnimUpdateMode(AnimatorUpdateMode.UnscaledTime);
                break;
            case State.Pause:
                ChangeAnimUpdateMode(AnimatorUpdateMode.Normal);
                break;
        }
    }
    void StateProcess()
    {
        switch (myState)
        {
            case State.Start:
                time_current = Time.time - time_start;
                if (time_current > time_Max) ChangeState(State.Play);
                break;
            case State.Play:
                break;
            case State.Pause:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CreatePhysicsScene();
        SimulateMovement();
        time_start = Time.time;
        Subscribe();
        ChangeState(State.Start);
    }

    // Update is called once per frame
    void Update()
    {
        StateProcess();
        SetBulletTime();

        //transform.position = ghostPlayer.position;
        //transform.rotation = ghostPlayer.rotation;

        _player.myCameras.myFPSCam.curRot = ghostCamera.myFPSCam.curRot;
        _player.myCameras.myTPSCam.curRot = ghostCamera.myTPSCam.curRot;
        _player.myCameras.myUICam.curRot = ghostCamera.myUICam.curRot;

        foreach (var mob in _spawnedMonsters)
        {
            mob.Value.transform.position = mob.Key.transform.position;
            mob.Value.transform.rotation = mob.Key.transform.rotation;
        }
        foreach (var item in _spawnedObjects)
        {
            //Debug.Log(_simulationScene + ", " + item.Value);
            item.Key.position = item.Value.position;
            item.Key.rotation = item.Value.rotation;
        }
        foreach (var item in _ghostInterables)
        {
            item.Value.GhostBehaviour(item.Key);
        }
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    #region BulletTime
    void ChangeAnimUpdateMode(AnimatorUpdateMode mode)
    {
        transform.GetComponent<Player>().ReturnAnim().updateMode = mode;
        ghostPlayer.GetComponent<Player>().ReturnAnim().updateMode = mode;
    }
    void SetBulletTime()
    {
        if (myState != State.Start) _physicsScene.Simulate(Time.fixedUnscaledDeltaTime);
        else _physicsScene.Simulate(Time.fixedDeltaTime);
    }
    void CreatePhysicsScene()
    {
        _simulationScene = SceneManager.CreateScene("Physics", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();

        foreach (Transform obj in _map)
        {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            var Renders = ghostObj.GetComponentsInChildren<Renderer>();
            var Interactables = obj.GetComponentsInChildren<ObjectNotGrabbable>();
            var ghostInteractables = ghostObj.GetComponentsInChildren<ObjectNotGrabbable>();
            var Canvases = ghostObj.GetComponentsInChildren<Canvas>();
            foreach (var r in Renders) r.enabled = false;
            foreach (var r in Canvases) r.enabled = false;

            for (int i = 0; i < Interactables.Length; ++i)
            {
                ghostInteractables[i].SetGhost(true);
                _ghostInterables.Add(Interactables[i], ghostInteractables[i]);
            }
            SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
            if (!ghostObj.isStatic) _spawnedObjects.Add(obj, ghostObj.transform);
        }
    }
    public void SimulateMovement()
    {
        _player.myCameras.transform.SetParent(_player.transform);
        // GhostPlayer Setting
        var ghostPlayer = Instantiate(_player);
        _player.myCameras.transform.SetParent(null);

        var Renders = ghostPlayer.GetComponentsInChildren<Renderer>();
        var Camera = ghostPlayer.GetComponentsInChildren<Camera>();
        var AudioListner = ghostPlayer.GetComponentsInChildren<AudioListener>();
        ghostPlayer.SetGhost(true);
        ghostPlayer.GetComponent<PlayerBulletTime>().enabled = false;
        ghostPlayer.GetComponent<Projection>().enabled = false;
        ghostPlayer.GetComponent<PlayerPickUpDrop>().enabled = false;
        ghostPlayer.myHPBar = null;
        ghostPlayer.myCameras.GhostSet(true);
        ghostPlayer.myCameras.SetPlayer(ghostPlayer);
        var AnimEvent = ghostPlayer.GetComponentsInChildren<AnimEvent>();

        foreach (var r in Renders) r.enabled = false;
        foreach (var c in Camera) c.enabled = false;
        foreach (var c in AudioListner) c.enabled = false;
        foreach (var c in AnimEvent) c.noSoundandEffect = true;

        // GhostMonsters Setting
        foreach (var mob in GameManagement.Inst.myMonsters)
        {
            var ghostMob = Instantiate(mob);
            Renders = ghostMob.GetComponentsInChildren<Renderer>();
            var scripts = ghostMob.GetComponents<MonoBehaviour>();

            foreach (var r in Renders) r.enabled = false;
            foreach (var script in scripts) script.enabled = false;

            var collider = ghostMob.GetComponent<CapsuleCollider>();
            mob.GetComponent<RagDollAction>().SetGhostCS(collider);

            SceneManager.MoveGameObjectToScene(ghostMob.gameObject, _simulationScene);
            _spawnedMonsters.Add(mob, ghostMob);
        }

        foreach (var item in _ghostInterables) item.Value.GhostBehaviour();

        SceneManager.MoveGameObjectToScene(ghostPlayer.gameObject, _simulationScene);
        if (!ghostPlayer.gameObject.isStatic) _spawnedObjects.Add(_player.transform, ghostPlayer.transform);
        this.ghostPlayer = ghostPlayer.transform;

        ghostCamera = ghostPlayer.myCameras;
        ghostCamera.transform.SetParent(null);

        if (!ghostCamera.myRoot.gameObject.isStatic) _spawnedObjects.Add(_player.myCameras.myRoot, ghostCamera.myRoot);
        if (!ghostCamera.mySpring.gameObject.isStatic) _spawnedObjects.Add(_player.myCameras.mySpring, ghostCamera.mySpring);
        if (!ghostCamera.myUI_basePos.gameObject.isStatic) _spawnedObjects.Add(_player.myCameras.myUI_basePos, ghostCamera.myUI_basePos);
    }

    public void OnEvent(GameStatesEvent eventType)
    {
        switch (eventType.gameEventType)
        {
            case GameEventType.Pause:
                ChangeState(State.Pause);
                break;
            case GameEventType.UnPause:
                ChangeState(State.Play);
                break;
        }
    }

    public void Subscribe()
    {
        this.EventStartingListening<GameStatesEvent>();
    }

    public void Unsubscribe()
    {
        this.EventStopListening<GameStatesEvent>();
    }
    #endregion
}
