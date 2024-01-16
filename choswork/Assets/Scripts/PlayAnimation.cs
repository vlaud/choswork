using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class PlayAnimation : ActionNode
{
    public string animationName;
    bool hasState;
    protected override void OnStart() {
        var stateId = Animator.StringToHash(animationName);
        hasState = context.animator.HasState(0, stateId);
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (hasState)
        {
            context.animator.Play(animationName);
            return State.Success;
        }
        return State.Failure;
    }
}
