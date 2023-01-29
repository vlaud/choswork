using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public static GameManagement Inst = null;
    public Player myPlayer;
    public Monster myMonster;
    public Inventory myInventory;
    public SpringArms mySpringArms;
    public SoundManager mySound;
    public MapManager myMapManager;
    [Range(0.01f, 1f)]
    public float GameTimeScale = 1f;
    [Range(0f, 0.02f)]
    public float GameFixedTimeScale = 0.02f;
    private void Awake()
    {
        Inst = this;
    }
    private void Update()
    {
        DoSlowmotion();
    }
    public void DoSlowmotion()
    {
        GameTimeScale = Mathf.Clamp(GameTimeScale, 0.01f, 1f);
        Time.timeScale = GameTimeScale;
        GameFixedTimeScale = Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
}
