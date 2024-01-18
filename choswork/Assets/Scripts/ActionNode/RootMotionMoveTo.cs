using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

[System.Serializable]
public class RootMotionMoveTo : ActionNode
{
    Movement movement;
    protected override void OnStart() {
        movement = context.gameObject.GetComponent<Movement>();
        Debug.Log(context.characterMovement);
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        context.characterMovement.RePath(movement.myPath, blackboard.Target.position, movement.filter);
        return State.Success;
    }
}
