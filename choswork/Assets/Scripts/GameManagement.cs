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
    public static GameManagement Inst = null;
    public Player myPlayer;
    public Monster myMonster;
    public Inventory myInventory;
    public SpringArms mySpringArms;
    public SoundManager mySound;
    public MapManager myMapManager;
    public SceneLoader mySceneLoader;
    public Mainmenu myMainmenu;
    public Keypad myKeypad;
    public Canvas myCanvas;
    public TMPro.TMP_Text myActionText;

    [Range(0.01f, 1f)]
    public float GameTimeScale = 1f;
    [Range(0f, 0.02f)]
    public float GameFixedTimeScale = 0.02f;
    public bool IsGameClear = false;
    private float timer;
    private bool IsBulletTime = false;
    private void Awake()
    {
        Inst = this;
        Physics.autoSimulation = false;
        mySceneLoader = GameObject.Find("SceneLoader")?.GetComponent<SceneLoader>();
        myMainmenu = mySceneLoader?.gameObject.GetComponentInChildren<Mainmenu>();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        while (timer >= Time.fixedDeltaTime)
        {
            timer -= Time.fixedDeltaTime;
            Physics.Simulate(Time.fixedDeltaTime);
        }
        DoSlowmotion();
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetBulletTime(0.3f, 5f);
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
        StartCoroutine(BulletTime(SetScale, Cooltime));
    }
    IEnumerator BulletTime(float SetScale, float Cooltime)
    {
        GameTimeScale = SetScale;
        Debug.Log("Slow Time Start: " + Cooltime + "sec");
        yield return new WaitForSecondsRealtime(Cooltime);
        GameTimeScale = 1f;
        Debug.Log("Slow Time End");
        IsBulletTime = false;
    }
}
