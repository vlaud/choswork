using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    public static GameManagement Inst = null;
    public Player myPlayer;
    public Inventory myInventory;
    public SpringArms mySpringArms;
    public SoundManager mySound;

    private void Awake()
    {
        Inst = this;
    }
}
