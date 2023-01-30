using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerBulletTime : MonoBehaviour
{
    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    [SerializeField] private Transform _map;
    private Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();
    private float time_start;
    private float time_current;
    private float time_Max = 1f;
    // Start is called before the first frame update
    void Start()
    {
        CreatePhysicsScene();
        time_start = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        time_current = Time.time - time_start;
        if (time_current > time_Max)
        {
            Physics.Simulate(Time.fixedUnscaledDeltaTime);
        }
        else Physics.Simulate(Time.fixedDeltaTime);

        foreach (var item in _spawnedObjects)
        {
            item.Key.position = item.Value.position;
            item.Key.rotation = item.Value.rotation;
        }
    }
    void CreatePhysicsScene()
    {
        _simulationScene = SceneManager.CreateScene("Physics", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();
        foreach (Transform obj in _map)
        {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            var Renders = ghostObj.GetComponentsInChildren<Renderer>();
            foreach (var r in Renders)
            {
                r.enabled = false;
            }
            SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
            if (!ghostObj.isStatic) _spawnedObjects.Add(obj, ghostObj.transform);
        }
    }
}
