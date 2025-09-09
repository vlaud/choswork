using System.Collections;
using System.Linq;
using UnityEngine;
using Project.Tools.InterfaceHelp;

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
    [CustomHeader("게임 상태")]
    public GameState myGameState = GameState.Create;
    [SerializeField] private GameState prevGameState = GameState.Create;
    public bool IsGameClear = false;
    public bool IsCutscene = false;
    private static GameManagement _inst = null;
    public static GameManagement Inst => _inst;

    [CustomHeader("플레이어 관련")]
    public Player myPlayer;
    
    [CustomHeader("몹 관련")]
    public AIPerception[] myMonsters;

    [CustomHeader("게임 요소 관련")]
    public SpringArms mySpringArms;
    public Inventory myInventory;
    public SoundManager mySound;
    public MapManager myMapManager;
    public SceneLoader mySceneLoader;
    public Mainmenu myMainmenu;
    public Canvas myCanvas;
    public TMPro.TMP_Text myActionText;
    public InterfaceHolder<iTimeFunctionality> myTimeManager; // TimeManager 참조 추가

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
                // TimeManager를 통해 시간 조절
                myTimeManager?.Value?.UnPause();
                // 재개 시 물리 상태 초기화
                Physics.SyncTransforms();
                break;
            case GameState.FadeToLevel:
                myMainmenu?.FadeToLevel();
                break;
            case GameState.Pause:
                // TimeManager를 통해 시간 조절
                myTimeManager?.Value?.Pause();

                // 일시정지 시 물리 상태 초기화
                Physics.SyncTransforms();
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

    private void Awake()
    {
        _inst = this;
        // TimeManager 인스턴스 찾기 또는 생성
        myTimeManager.SetValue(ComponentTypeFinder.FindFirstImplementing<iTimeFunctionality>());

        myMonsters = FindObjectsByType<AIPerception>(FindObjectsSortMode.None);
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

    public void GameClear()
    {
        if (IsGameClear) ChangeState(GameState.FadeToLevel);
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }
}
