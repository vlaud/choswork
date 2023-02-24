using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ItemEvent
{
    void SetItemEvent();
    void SetItemTargetObj(Transform target);
}
public class GameManagement : MonoBehaviour
{
    public enum GameState
    {
        Create, Play, Pause, GameOver
    }
    public GameState myGameState = GameState.Create;
    public static GameManagement Inst = null;
    public Player myPlayer;
    //public Monster myMonster;
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
        myGameState = s;

        switch(myGameState)
        {
            case GameState.Play:
                IsBulletTime = false;
                if (curBulletTime > Mathf.Epsilon) SetBulletTime(desireScale, curBulletTime);
                else GameTimeScale = 1f;
                break;
            case GameState.Pause:
                StopAllCoroutines();
                GameTimeScale = 0.01f;
                break;
            case GameState.GameOver:
                StopAllCoroutines();
                if(myMainmenu != null)
                {
                    myMainmenu.transform.parent.SetParent(null);
                    myMainmenu.newGameSceneName = "Title";
                    myMainmenu?.FadeToLevel();
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
            case GameState.Pause:
                break;
            case GameState.GameOver:
                break;
        }
    }
    private void Awake()
    {
        Inst = this;
        Physics.autoSimulation = IsCutscene;
        myMonsters = FindObjectsOfType(typeof(AIPerception)) as AIPerception[];
        for(int i =0; i < myMonsters.Length; ++i)
        {
            myMonsters[i].GetComponent<AIAction>().SetMobIndex(i);
        }
        mySceneLoader = GameObject.Find("SceneLoader")?.GetComponent<SceneLoader>();
        myMainmenu = mySceneLoader?.gameObject.GetComponentInChildren<Mainmenu>();
        if(myMainmenu != null && myCanvas != null)
        {
            myCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            myCanvas.worldCamera = myMainmenu.transform.parent.GetComponent<Camera>();
        }
        ChangeState(GameState.Play);
    }
    private void Update()
    {
        if(!IsCutscene)
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
    public void DoSlowmotion()
    {
        GameTimeScale = Mathf.Clamp(GameTimeScale, 0.01f, 1f);
        Time.timeScale = GameTimeScale;
        GameFixedTimeScale = Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
    public void GameClear()
    {
        if (IsGameClear)
            myMainmenu?.FadeToLevel();
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
    public void PauseGame()
    {
        ChangeState(GameState.Pause);
    }
    public void UnPauseGame()
    {
        ChangeState(GameState.Play);
    }
    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }
}
