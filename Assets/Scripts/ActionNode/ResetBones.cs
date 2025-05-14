using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class ResetBones : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (blackboard.movement.rdState == RagDollState.StandUp)
        {
            return State.Success;
        }
        if (blackboard.movement.rdState == RagDollState.ResetBones)
        {
            blackboard.movement.ResetBonesBehaviour();
            return State.Running;
        }
        return State.Failure;
    }
}
