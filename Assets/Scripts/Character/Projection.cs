using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoBehaviour
{
    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    [SerializeField] private Transform _map;
    [SerializeField] private GameObject _ghostobj;
    private Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();

    private Coroutine _simulate;

    // Start is called before the first frame update
    void Start()
    {
        CreatePhysicsScene();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var item in _spawnedObjects)
        {
            item.Value.position = item.Key.position;
            item.Value.rotation = item.Key.rotation;
        }
    }

    void CreatePhysicsScene()
    {
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();
        foreach (Transform obj in _map)
        {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            var Renders = ghostObj.GetComponentsInChildren<Renderer>();
            var Canvases = ghostObj.GetComponentsInChildren<Canvas>();
            var ghostInteractables = ghostObj.GetComponentsInChildren<ObjectNotGrabbable>();
            foreach (var r in Renders) r.enabled = false;
            foreach (var r in Canvases) r.enabled = false;
            foreach (var r in ghostInteractables)
            {
                r.SetGhost(true);
                r.GhostBehaviour();
            }
            SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
            if (!ghostObj.isStatic) _spawnedObjects.Add(obj, ghostObj.transform);
        }
    }
    [SerializeField] private LineRenderer _line;
    [Range(1, 100)]
    [SerializeField] private int _maxPhysicsFrameIterations;
    [Range(0.1f, 2f)]
    [SerializeField] private float _timeOffset;
    public bool IsSimulation = false;


    public void StopSimultation()
    {
        this.StopCurrentCoroutine(ref _simulate);
        Debug.Log($"_simulate: {_simulate}, _simulate is Null? : {_simulate == null }");
        IsSimulation = false;
    }

    public void SetSimulation(ObjectGrabbable objGrab, Vector3 pos, Vector3 dir, float strength)
    {
        if (IsSimulation) return;

        this.StartOrRestartCoroutine(ref _simulate, SimulateTrajectory(objGrab, pos, dir, strength));
    }

    IEnumerator SimulateTrajectory(ObjectGrabbable objGrab, Vector3 pos, Vector3 dir, float strength)
    {
        IsSimulation = true;

        while (IsSimulation && objGrab != null)
        {
            if (objGrab == null) break;

            if (_ghostobj == null)
            {
                _ghostobj = Instantiate(Resources.Load("Prefabs/Cup"), pos, Quaternion.identity) as GameObject;

                var Renders = _ghostobj.GetComponentsInChildren<Renderer>();
                foreach (var r in Renders)
                {
                    r.enabled = false;
                }
                SceneManager.MoveGameObjectToScene(_ghostobj.gameObject, _simulationScene);
            }

            if (!_ghostobj.activeSelf) _ghostobj.SetActive(true);

            //TODO: objGrab = null 방지
            Debug.Log($"objGrab: {objGrab}");

            _ghostobj.transform.position = objGrab.transform.position;
            _ghostobj.transform.rotation = Quaternion.identity;

            if (transform.TryGetComponent<PlayerPickUpDrop>(out var pp))
            {
                dir = pp.GetobjectGrabPointForward();
            }

            _ghostobj.GetComponent<ObjectGrabbable>().Throw(dir, strength, true);
            _line.positionCount = Mathf.CeilToInt(_maxPhysicsFrameIterations / _timeOffset) + 1;
            int i = 0;
            _line.SetPosition(i, _ghostobj.transform.position);

            for (float time = 0; time < _maxPhysicsFrameIterations; time += _timeOffset)
            {
                i++;
                _physicsScene.Simulate(Time.fixedDeltaTime);
                _line.SetPosition(i, _ghostobj.transform.position);
            }

            _ghostobj.SetActive(false);

            yield return null;
        }
    }
}
