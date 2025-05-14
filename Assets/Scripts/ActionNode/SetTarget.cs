using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class SetTarget : ActionNode
{
    protected override void OnStart() {
        blackboard.movement.IsStart = !blackboard.movement.IsStart;
        MapManager.Inst.MobChangePath(blackboard.movement.IsStart, blackboard.movement.mobIndex);
        blackboard.movement.FindTarget(MapManager.Inst.GetDestination(blackboard.movement.IsStart, blackboard.movement.mobIndex));
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        blackboard.Target = blackboard.movement.GetMyTarget();
        return State.Success;
    }
}
