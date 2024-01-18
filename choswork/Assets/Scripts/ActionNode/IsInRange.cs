using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class IsInRange : ActionNode
{
    Movement movement;
    protected override void OnStart() {
        movement = context.gameObject.GetComponent<Movement>();
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
