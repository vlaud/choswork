using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class SettingNode : GenericTypeActionNode<Movement>
{
    protected override void OnStart() {
        value = blackboard.movement = context.gameObject.GetComponent<Movement>();
        //context.transform.position = GameManagement.Inst.myMapManager.GetDestination(false, value.mobIndex).position;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (blackboard.movement == null) 
            return State.Success;
        return State.Failure;
    }
}
