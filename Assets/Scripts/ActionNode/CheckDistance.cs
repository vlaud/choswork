using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class CheckDistance : ActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (CheckDist() <= blackboard.movement.myStat.AttackRange)
        {
            return State.Success;
        }
        return State.Failure;
    }

    float CheckDist()
    {
        Vector3 dir = blackboard.movement.GetMyTarget().position - context.transform.position;
        float dist = dir.magnitude;
        return dist;
    }
}
