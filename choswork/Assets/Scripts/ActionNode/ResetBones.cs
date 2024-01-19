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
        if (blackboard.rdState == RagDollState.StandUp)
        {
            return State.Success;
        }
        if (blackboard.rdState == RagDollState.ResetBones)
        {
            blackboard.movement.ResetBonesBehaviour();
            return State.Running;
        }


        return State.Failure;
    }
}
