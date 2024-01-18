using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEngine.AI;

[System.Serializable]
public class RootMotionMoveTo : ActionNode
{
    private NavMeshPath myPath;
    private NavMeshQueryFilter filter;
    private MonsterMovement movement;
    protected override void OnStart() {
        myPath = new NavMeshPath();
        filter.areaMask = 1 << GameManagement.Inst.myMapManager.surfaces.defaultArea;
        filter.agentTypeID = GameManagement.Inst.myMapManager.surfaces.agentTypeID;
        Debug.Log(context.characterMovement);
        movement = context.characterMovement as MonsterMovement;
        blackboard.Target = movement.player;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        movement.RePath(myPath, blackboard.Target.position, filter);
        return State.Running;
    }
}
