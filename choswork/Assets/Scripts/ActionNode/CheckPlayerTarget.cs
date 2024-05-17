using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class CheckPlayerTarget : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (blackboard.movement.GetMyTarget() == blackboard.movement.Player)
        {
            return State.Success;
        }
        return State.Failure;
    }
}
