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
    // Start is called before the first frame update
    void Start()
    {
        CreatePhysicsScene();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var item in _spawnedObjects)
        {
            item.Value.position = item.Key.position;
            item.Value.rotation = item.Key.rotation;
        }
    }
    void CreatePhysicsScene()
    {
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();
        foreach(Transform obj in _map)
        {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            var Renders = ghostObj.GetComponentsInChildren<Renderer>();
            foreach(var r in Renders)
            {
                r.enabled = false;
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
    public void SimulateTrajectory(ObjectGrabbable objGrab, Vector3 pos, Vector3 dir, float strength)
    {
        if(_ghostobj == null)
        {
            _ghostobj = Instantiate(objGrab.gameObject, pos, Quaternion.identity);

            var Renders = _ghostobj.GetComponentsInChildren<Renderer>();
            foreach (var r in Renders)
            {
                r.enabled = false;
            }
            SceneManager.MoveGameObjectToScene(_ghostobj.gameObject, _simulationScene);
        }
        if (!_ghostobj.activeSelf) _ghostobj.SetActive(true);

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
        _ghostobj.transform.position = pos;
        _ghostobj.transform.rotation = Quaternion.identity;
        //Destroy(ghostObj.gameObject); 
    }
}
