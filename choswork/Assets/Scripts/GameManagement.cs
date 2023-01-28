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
    [Range(0f, 1f)]
    public float GameTimeScale = 1f;
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
        Time.timeScale = GameTimeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
}
