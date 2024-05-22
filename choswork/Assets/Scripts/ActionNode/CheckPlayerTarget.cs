using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class CheckPlayerTarget : ActionNode
{
    public LayerMask targetMask;
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (blackboard.movement.GetMyTarget() == null) return State.Failure;

        if ((targetMask & 1 << blackboard.movement.GetMyTarget()?.gameObject.layer) != 0)
        {
            return State.Success;
        }
        return State.Failure;
    }
}
