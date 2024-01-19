using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class GetTarget : GenericTypeActionNode<Movement>
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        value = context.gameObject.GetComponent<Movement>();
        blackboard.Target = value.target;
        Debug.Log(value.target);
        if (value.target != null)
        {
            return State.Success;
        }
        return State.Failure;
    }
}
