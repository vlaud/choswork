using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class GetTarget : ActionNode
{
    Movement movement;
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        movement = context.gameObject.GetComponent<Movement>();
        blackboard.Target = movement.target;
        Debug.Log(movement.target);
        if (movement.target != null)
        {
            return State.Success;
        }
        return State.Failure;
    }
}
