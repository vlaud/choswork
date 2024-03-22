using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class PlayAnimation : ActionNode
{
    public MovementState DesiredState;
    public string animationName;
    public bool SuccessAfterAnimation = false;
    public bool PassPrevAnimationChecking = false;
    bool hasState;
    protected override void OnStart() {
        var stateId = Animator.StringToHash(animationName);
        hasState = context.animator.HasState(0, stateId);

        if (hasState)
        {
            blackboard.movement.ChangeState(DesiredState);
            context.animator.CrossFade(animationName, 0.2f);
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if(!PassPrevAnimationChecking)
        {
            if (!context.animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
                return State.Running;
        }
        if (hasState)
        {
            Debug.Log(animationName + ": " + AnimatorIsPlaying(animationName));
            if (!SuccessAfterAnimation)
                return State.Success;
            
            if(AnimatorIsDone(animationName))
                return State.Success;

            return State.Running;
        }
        return State.Failure;
    }

    bool AnimatorIsDone()
    {
        return context.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1;
    }

    bool AnimatorIsDone(string stateName)
    {
        return AnimatorIsDone() && context.animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    bool AnimatorIsPlaying() 
    { 
        return context.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1; 
    }

    bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && context.animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
}
