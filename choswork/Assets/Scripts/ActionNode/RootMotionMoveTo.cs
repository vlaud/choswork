using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

[System.Serializable]
public class RootMotionMoveTo : GenericTypeActionNode<Movement>
{
    protected override void OnStart() {
        value = context.gameObject.GetComponent<Movement>();
        Debug.Log(context.characterMovement);
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        context.characterMovement.RePath(value.myPath, blackboard.Target.position, value.filter);
        return State.Success;
    }
}
