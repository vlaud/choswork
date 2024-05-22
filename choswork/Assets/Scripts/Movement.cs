using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MovementState
{
    Idle, Roaming, Angry, Search
}
public class Movement : RagDollAction, AIAction
{
    public bool IsStart = false;
    public int mobIndex;
    public Transform target;

    //ai path
    public NavMeshPath myPath;
    public NavMeshQueryFilter filter;

    public AIState aiState;
    public RagDollState rdState;
    public MovementState state;

    private void Awake()
    {
        cs = GetComponent<CapsuleCollider>();
        _origintimetoWake = _timetoWakeup;
        _bones = myHips.GetComponentsInChildren<Transform>();
        _faceUpStandUpBoneTransforms = new BoneTransform[_bones.Length];
        _faceDownStandUpBoneTransforms = new BoneTransform[_bones.Length];
        _ragdollTransforms = new BoneTransform[_bones.Length];

        for (int boneIndex = 0; boneIndex < _bones.Length; ++boneIndex)
        {
            _faceUpStandUpBoneTransforms[boneIndex] = new BoneTransform();
            _faceDownStandUpBoneTransforms[boneIndex] = new BoneTransform();
            _ragdollTransforms[boneIndex] = new BoneTransform();
        }
        PopulateAnimation(_faceUpStandUpClipName, _faceUpStandUpBoneTransforms);
        PopulateAnimation(_faceDownStandUpClipName, _faceDownStandUpBoneTransforms);
        RagDollSet(false);
    }
    private void Start()
    {
        myPath = new NavMeshPath();
        filter.areaMask = 1 << GameManagement.Inst.myMapManager.surfaces.defaultArea;
        filter.agentTypeID = GameManagement.Inst.myMapManager.surfaces.agentTypeID;
        rdState = RagDollState.NoRagdoll;
        ChangeState(MovementState.Idle);
    }

    private void Update()
    {
        myAnim.speed = myStat.MoveSpeed;
        target = myTarget;
    }

    public void ChangeState(MovementState state)
    {
        if (this.state == state) return;
        this.state = state;

        Debug.Log(this.state);
    }

    public void FindPlayer(Transform target)
    {
        Debug.Log("Å¸°Ù È®º¸");
        aiState = AIState.Angry;
        FindTarget(target);
    }

    public void FindTarget(Transform target)
    {
        StopAllCoroutines();
        myTarget = target;
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
        return myTarget;
    }

    public void HearingSound()
    {
        Transform tempTarget = GameManagement.Inst.myPlayer.transform;
        if ((tempTarget != null && tempTarget.TryGetComponent<PlayerPickUpDrop>(out var target)))
        {
            if (target.GetObjectGrabbable() != null)
            {
                hearingObj = target.GetObjectGrabbable().transform;
                Debug.Log("hearingObj: " + hearingObj);
            }
            else if (hearingObj != null && hearingObj.TryGetComponent<ObjectGrabbable>(out var grab))
            {
                if (grab.IsSoundable)
                {
                    hearingPos = grab.soundPos;
                    CheckSoundDist(myPath, filter.areaMask,
                        LayerMask.NameToLayer("Ground"), filter, () => LostTarget());
                    //grab.IsSoundable = false;
                }
            }
        }
        if (aiHeardPlayer)
        {
            ChangeState(MovementState.Search);
        }
    }

    public bool IsSearchable()
    {
        return aiState == AIState.Normal;
    }

    public virtual void LostTarget()
    {
        myTarget = null;
        aiHeardPlayer = false;
        StopAllCoroutines();
        state = MovementState.Idle;
    }

    public void SetAnimTrigger()
    {

    }

    public void SetMobIndex(int mobIndex)
    {
        this.mobIndex = mobIndex;
    }

    public override void ChangeRagDollState(RagDollState ragdoll)
    {
        if (rdState == ragdoll) return;
        rdState = ragdoll;

        switch (rdState)
        {
            case RagDollState.ResetBones:
                break;
            case RagDollState.StandUp:
                myAnim.Play(GetStandUpStateName(), -1, 0.0f);
                break;
            case RagDollState.NoRagdoll:
                //FindTarget(myGamemanager.myPlayer.transform, STATE.Angry);
                break;
        }
    }
}
