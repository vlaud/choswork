using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerBulletTime : MonoBehaviour
{
    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    [SerializeField] private Transform _map;
    [SerializeField] private Player _player;
    private Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();
    private float time_start;
    private float time_current;
    private float time_Max = 1f;
    // Start is called before the first frame update
    void Start()
    {
        CreatePhysicsScene();
        SimulateMovement();
        time_start = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        time_current = Time.time - time_start;
        if (time_current > time_Max)
        {
            _physicsScene.Simulate(Time.fixedUnscaledDeltaTime);
        }
        else _physicsScene.Simulate(Time.fixedDeltaTime);

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
    public void SimulateMovement()
    {
        var ghostPlayer = Instantiate(_player);
        var Renders = ghostPlayer.GetComponentsInChildren<Renderer>();
        var Camera = ghostPlayer.GetComponentsInChildren<Camera>();
        var AudioListner = ghostPlayer.GetComponentsInChildren<AudioListener>();
        ghostPlayer.GetComponent<PlayerBulletTime>().enabled = false;
        ghostPlayer.GetComponent<Projection>().enabled = false;
        ghostPlayer.GetComponent<PlayerPickUpDrop>().enabled = false;
        var AnimEvent = ghostPlayer.GetComponentsInChildren<AnimEvent>();
        foreach (var r in Renders)
        {
            r.enabled = false;
        }
        foreach (var c in Camera)
        {
            c.enabled = false;
        }
        foreach (var c in AudioListner)
        {
            c.enabled = false;
        }
        foreach (var c in AnimEvent)
        {
            c.noSoundandEffect = true;
        }
        SceneManager.MoveGameObjectToScene(ghostPlayer.gameObject, _simulationScene);
        if (!ghostPlayer.gameObject.isStatic) _spawnedObjects.Add(_player.transform, ghostPlayer.transform);
        ghostPlayer.PlayerMove();
    }
}
