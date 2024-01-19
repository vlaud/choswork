using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

[System.Serializable]
public class RootMotionMoveTo : ActionNode
{
    protected override void OnStart() {
        context.characterMovement.RePath(blackboard.movement.myPath, 
            blackboard.movement.target.position, blackboard.movement.filter);
        Debug.Log(context.characterMovement);
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
