using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public interface ItemDesireEvent
{
    void SetItemEvent();
}
public interface ItemTargeting
{
    void SetItemTargetObj(Transform target);
}
public enum GameState
{
    Create, Play, FadeToLevel, Pause, GameOver
}

public class GameManagement : MonoBehaviour, iSubscription, EventListener<GameStatesEvent>
{
    [Header("게임 상태")]
    public GameState myGameState = GameState.Create;
    [SerializeField] private GameState prevGameState = GameState.Create;
    public bool IsGameClear = false;
    public bool IsCutscene = false;
    private static GameManagement _inst = null;
    public static GameManagement Inst => _inst;

    [Header("플레이어 관련")]
    public Player myPlayer;

    [Header("몹 관련")]
    public AIPerception[] myMonsters;
    [SerializeField]
    private Rigidbody[] rigidbodies;

    [Header("게임 요소 관련")]
    public SpringArms mySpringArms;
    public Inventory myInventory;
    public SoundManager mySound;
    public MapManager myMapManager;
    public SceneLoader mySceneLoader;
    public Mainmenu myMainmenu;
    public Canvas myCanvas;
    public TMPro.TMP_Text myActionText;

    [Header("시간 속도 설정")]
    [SerializeField] private float pauseTime = 0f;

    [Range(0f, 1f)]
    public float GameTimeScale = 1f;
    [Range(0f, 0.02f)]
    public float GameFixedTimeScale = 0.02f;
    
    private float timer;
    [SerializeField] private bool IsBulletTime = false;
    [SerializeField] private float curBulletTime;
    [SerializeField] private float desireScale;

    void ChangeState(GameState s)
    {
        if (myGameState == s) return;

        if (myGameState != GameState.Pause)
        {
            prevGameState = myGameState;
        }

        myGameState = s;

        switch (myGameState)
        {
            case GameState.Play:
                IsBulletTime = false;
                if (curBulletTime > Mathf.Epsilon) SetBulletTime(desireScale, curBulletTime);
                else GameTimeScale = 1f;
                break;
            case GameState.FadeToLevel:
                myMainmenu?.FadeToLevel();
                break;
            case GameState.Pause:
                StopAllCoroutines();
                GameTimeScale = pauseTime;
                break;
            case GameState.GameOver:
                StopAllCoroutines();
                if (myMainmenu != null)
                {
                    myMainmenu.transform.parent.SetParent(null);
                    myMainmenu.newGameSceneName = "Title";
                    ChangeState(GameState.FadeToLevel);
                }
                break;
        }
    }
    void StateProcess()
    {
        switch (myGameState)
        {
            case GameState.Play:
                if (IsBulletTime) curBulletTime -= Time.unscaledDeltaTime;
                break;
            case GameState.FadeToLevel:
                break;
            case GameState.Pause:
                break;
            case GameState.GameOver:
                break;
        }
    }
    private void Awake()
    {
        _inst = this;
        Physics.simulationMode = SimulationMode.Script;
        myMonsters = FindObjectsByType<AIPerception>(FindObjectsSortMode.None);
        GetAllMobsRigidbodies();
        for (int i = 0; i < myMonsters.Length; ++i)
        {
            myMonsters[i].GetComponent<AIAction>().SetMobIndex(i);
        }
        mySceneLoader = GameObject.Find("SceneLoader")?.GetComponent<SceneLoader>();

        if (mySceneLoader?.gameObject.GetComponentInChildren<Mainmenu>() != null)
            myMainmenu = mySceneLoader?.gameObject.GetComponentInChildren<Mainmenu>();
        else
            myMainmenu = FindFirstObjectByType(typeof(Mainmenu)) as Mainmenu;

        if (myMainmenu != null && myCanvas != null)
        {
            myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            myCanvas.worldCamera = myMainmenu.transform.parent.GetComponent<Camera>();
        }

        ChangeState(GameState.Play);
        prevGameState = GameState.Play;
    }

    private void Start()
    {
        Subscribe();
    }

    private void Update()
    {
        if (!IsCutscene)
        {
            timer += Time.deltaTime;
            while (timer >= Time.fixedDeltaTime)
            {
                timer -= Time.fixedDeltaTime;
                // GameTimeScale이 0이 아닐 때만 물리 시뮬레이션 실행
                if (GameTimeScale > 0)
                {
                    Physics.Simulate(Time.fixedDeltaTime * GameTimeScale);
                }
            }
            DoSlowmotion();
            StateProcess();
        }
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void OnEvent(GameStatesEvent eventType)
    {
        switch (eventType.gameEventType)
        {
            case GameEventType.Pause:
                CursorManager.Instance.SetPaused(true);
                ChangeState(GameState.Pause);
                break;
            case GameEventType.UnPause:
                CursorManager.Instance.SetPaused(false);
                ChangeState(prevGameState);
                break;
        }
    }

    public void Subscribe()
    {
        if (myMainmenu?.CurrentSceneName == "Title") return;
        this.EventStartingListening<GameStatesEvent>();
    }

    public void Unsubscribe()
    {
        if (myMainmenu?.CurrentSceneName == "Title") return;
        this.EventStopListening<GameStatesEvent>();
    }

    private void GetAllMobsRigidbodies()
    {
        rigidbodies = myMonsters
            .Select(monster => monster.GetComponent<RagDollAction>()) // RagDollAction 찾기
            .Where(ragDoll => ragDoll != null) // Null 제거
            .SelectMany(ragDoll => ragDoll.myRagDolls.GetAllRigidbodies()) // 모든 Rigidbody 가져오기
            .ToArray();

        Debug.Log($"맵에 있는 Rigidbody 개수: {rigidbodies.Length}");
    }

    public void DoSlowmotion()
    {
        GameTimeScale = Mathf.Clamp(GameTimeScale, 0f, 1f);
        // 현재 존재하는 모든 Ragdoll Rigidbody 찾기

        Time.timeScale = GameTimeScale;
        GameFixedTimeScale = Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
    public void GameClear()
    {
        if (IsGameClear) ChangeState(GameState.FadeToLevel);
    }
    public void SetBulletTime(float SetScale, float Cooltime)
    {
        if (IsBulletTime) return;
        IsBulletTime = true;
        curBulletTime = Cooltime;
        desireScale = SetScale;
        StartCoroutine(BulletTime(SetScale, Cooltime));
    }
    IEnumerator BulletTime(float SetScale, float Cooltime)
    {
        GameTimeScale = SetScale;
        Debug.Log("Slow Time Start: " + Cooltime + "sec");
        myPlayer.TimeStopCheck(true);
        yield return new WaitForSecondsRealtime(Cooltime);
        GameTimeScale = 1f;
        Debug.Log("Slow Time End");
        IsBulletTime = false;
        myPlayer.TimeStopCheck(false);
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    public bool GetIsBulletTime()
    {
        return IsBulletTime;
    }
}
