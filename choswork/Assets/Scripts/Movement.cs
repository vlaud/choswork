using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : RagDollAction, AIAction
{
    public Transform target;
    public int mobIndex;
    public AIState aiState;
    //ai path
    public NavMeshPath myPath;
    public NavMeshQueryFilter filter;

    private void Start()
    {
        myPath = new NavMeshPath();
        filter.areaMask = 1 << GameManagement.Inst.myMapManager.surfaces.defaultArea;
        filter.agentTypeID = GameManagement.Inst.myMapManager.surfaces.agentTypeID;
    }
    public void FindPlayer(Transform target)
    {
        Debug.Log("Å¸°Ù È®º¸");
        aiState = AIState.Angry;
        this.target = target;
    }

    public AIState GetAIState()
    {
        return aiState;
    }

    public int GetMobIndex()
    {
        return mobIndex;
    }

    public NavMeshPath GetMyPath()
    {
        return myPath;
    }

    public Transform GetMyTarget()
    {
        return target;
    }

    public void HearingSound()
    {

    }

    public bool IsSearchable()
    {
        return aiState == AIState.Normal;
    }

    public virtual void LostTarget()
    {

    }

    public void SetAnimTrigger()
    {

    }

    public void SetMobIndex(int mobIndex)
    {
        this.mobIndex = mobIndex;
    }
}
