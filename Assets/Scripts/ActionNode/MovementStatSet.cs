using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class MovementStatSet : ActionNode
{
    protected override void OnStart() {
        blackboard.Target = blackboard.movement.GetMyTarget();
        blackboard.State = blackboard.movement.state;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
