using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class RagdollBehave : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (blackboard.movement.rdState == RagDollState.ResetBones)
        {
            return State.Failure;
        }
        if (context.myRagDolls.isRagdoll)
        {
            blackboard.movement.RagdollBehaviour();
            return State.Running;
        }
        return State.Failure;
    }
}
