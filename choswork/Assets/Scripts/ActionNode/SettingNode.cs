using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class SettingNode : GenericTypeActionNode<Movement>
{
    protected override void OnStart() {
        value = context.gameObject.GetComponent<Movement>();
        blackboard.movement = value;
        blackboard.Target = blackboard.movement.target;
        blackboard.aiState = blackboard.movement.aiState;
        blackboard.rdState = blackboard.movement.rdState;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if(blackboard.movement == null) 
            return State.Success;
        return State.Failure;
    }
}
