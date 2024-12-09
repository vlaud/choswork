using System.Collections;
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

    public GameState myGameState = GameState.Create;
    [SerializeField] private GameState prevGameState = GameState.Create;

    private static GameManagement _inst = null;
    public static GameManagement Inst => _inst;
    public Player myPlayer;
    public AIPerception[] myMonsters;
    public SpringArms mySpringArms;
    public Inventory myInventory;
    public SoundManager mySound;
    public MapManager myMapManager;
    public SceneLoader mySceneLoader;
    public Mainmenu myMainmenu;
    public Canvas myCanvas;
    public TMPro.TMP_Text myActionText;

    [Range(0.01f, 1f)]
    public float GameTimeScale = 1f;
    [Range(0f, 0.02f)]
    public float GameFixedTimeScale = 0.02f;
    public bool IsGameClear = false;
    public bool IsCutscene = false;
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
                GameTimeScale = 0.01f;
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
        myMonsters = FindObjectsByType(typeof(AIPerception), FindObjectsSortMode.None) as AIPerception[];
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
                Physics.Simulate(Time.fixedDeltaTime);
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
        //Debug.Log(myMainmenu?.CurrentSceneName + ": game Sub");
    }

    public void Unsubscribe()
    {
        if (myMainmenu?.CurrentSceneName == "Title") return;
        this.EventStopListening<GameStatesEvent>();
        //Debug.Log("game unSub");
    }

    public void DoSlowmotion()
    {
        GameTimeScale = Mathf.Clamp(GameTimeScale, 0.01f, 1f);
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
