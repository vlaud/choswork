using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class StandUp : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            blackboard.movement.ChangeRagDollState(RagDollState.NoRagdoll);
            return State.Success;
        }
        if (blackboard.rdState == RagDollState.StandUp)
        {
            return State.Running;
        }

        return State.Failure;
    }
}
