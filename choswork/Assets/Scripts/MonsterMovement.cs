using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : Movement
{
    public override void LostTarget()
    {
        aiState = AIState.Normal;
        Debug.Log("Ÿ�� ��ħ");
        target = null;
    }
}
