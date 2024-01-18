using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class GetTarget : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        Movement movement = context.characterMovement as Movement;
        blackboard.Target = movement.player;
        return State.Success;
    }
}
