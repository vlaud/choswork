using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Crawler : RagDollAction, AIAction
{
    private GameManagement myGamemanager;
    public LayerMask enemyMask = default;
    public Transform mobTarget; // for checking myTarget
    public bool IsGameOver = false;

    //RotReversing
    public Transform myReverser;
    bool IsGround = false;
    public float JumpPower = 10f;

    //old cs = r: 0.33, h: 1.21
    //new cs = r: 0.15, h: 0.7

    //mob startPos
    public bool IsStart = false;
    public int mobIndex;

    //ai path
    private NavMeshPath myPath;
    NavMeshQueryFilter filter;

    Vector3[] drawings = { Vector3.zero, Vector3.zero};
    public enum STATE
    {
        Create, Idle, Roaming, Search, Angry,
        ToGround, ToJump, ToCeiling, ToAir,
        RagDoll, StandUp, ResetBones, Death
    }
    public STATE myState = STATE.Create;
    public STATE formerState = STATE.Create;
    void ChangeState(STATE s)
    {
        if (myState == s) return;
        myState = s;

        switch (myState)
        {
            case STATE.Create:
                break;
            case STATE.Idle: // 평상시
                formerState = STATE.Idle;
                IsStart = !IsStart;
                myGamemanager.myMapManager.CrawlerChangePath(IsStart);
                FindTarget(myGamemanager.myMapManager.GetCrDestination(IsStart), STATE.Idle);
                StartCoroutine(DelayState(STATE.Roaming, _changeStateTime));
                break;
            case STATE.Roaming:
                formerState = STATE.Roaming;
                RePath(myPath, myTarget, filter, () => LostTarget());
                CorrectBaseHeight(myPath, myTarget, filter);
                break;
            case STATE.Angry:
                formerState = STATE.Angry;
                myAnim.SetBool("IsMoving", false); // 움직임 비활성화
                AttackTarget(myPath, myTarget, filter);
                break;
            case STATE.Search:
                if (formerState == STATE.Search) RePath(myPath, myTarget, filter, () => LostTarget());
                else formerState = STATE.Search;
                if (TrackSoundFailed(myPath)) LostTarget();
                myAnim.SetBool("IsMoving", false);
                myAnim.SetTrigger("Search");
                break;
            case STATE.ToGround:
                IsGround = true;
                myRigid.useGravity = true;
                ChangeState(STATE.RagDoll);
                break;
            case STATE.ToJump:
                StopAllCoroutines();
                myRigid.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
                break;
            case STATE.ToAir:
                break;
            case STATE.ToCeiling:
                myReverser.localRotation = Quaternion.Euler(new Vector3(180f, 180f, 0f));
                myReverser.localPosition = new Vector3(0f, 0.36f, 0f);
                IsGround = false;
                myRigid.useGravity = false;
                ChangeState(formerState);
                break;
            case STATE.RagDoll:
                StopAllCoroutines();
                myReverser.localRotation = Quaternion.identity;
                myReverser.localPosition = Vector3.zero;
                RagDollSet(true);
                break;
            case STATE.StandUp:
                myAnim.Play(_faceUpStandUpStateName, -1, 0.0f);
                break;
            case STATE.ResetBones:
                break;
            case STATE.Death:
                break;
        }
    }
    void StateProcess()
    {
        mobTarget = myTarget;
        switch (myState)
        {
            case STATE.Create:
                break;
            case STATE.Idle:
                break;
            case STATE.Roaming:
                SetCeilingOrFloor();
                break;
            case STATE.Angry:
                myAnim.SetBool("IsAngry", true);
                break;
            case STATE.Search:
                SetCeilingOrFloor();
                break;
            case STATE.RagDoll:
                RagdollBehaviour();
                break;
            case STATE.StandUp:
                StandingUpBehaviour();
                break;
            case STATE.ResetBones:
                ResetBonesBehaviour();
                break;
            case STATE.Death:
                break;
        }
    }
    public int GetAgentId(string agentName)
    {
        var count = NavMesh.GetSettingsCount();
        for (var i = 0; i < count; i++)
        {
            var id = NavMesh.GetSettingsByIndex(i).agentTypeID;
            var name = NavMesh.GetSettingsNameFromID(id);
            if (agentName == name)
                return id;
        }
        return -1;
    }
    void SetCeilingOrFloor()
    {
        if (myPath.corners.Length < 3) return;
        Vector3 dir = myPath.corners[2] - myPath.corners[1];
        float dist = dir.magnitude;
        LayerMask mask;
        STATE desireState;
        Debug.DrawLine(myPath.corners[1], myPath.corners[2], Color.green);
        if (!IsGround)
        {
            mask = LayerMask.NameToLayer("Ground");
            desireState = STATE.ToGround;
        }
        else
        {
            mask = LayerMask.NameToLayer("Ceiling");
            desireState = STATE.ToJump;
        }
        Debug.Log(desireState.ToString());

        if (Physics.Raycast(myPath.corners[1], dir,
           out RaycastHit thit, dist + 1f, 1 << mask))
        {
            Debug.Log(myPath.corners[1]);
            Debug.Log(thit.point);
            drawings[0] = myPath.corners[1];
            drawings[1] = thit.point;
            Debug.Log(LayerMask.LayerToName(thit.transform.gameObject.layer));
            ChangeState(desireState);
        }
    }

    void DrawPath()
    {
        LayerMask mask = LayerMask.GetMask("Ceiling") | LayerMask.GetMask("Ground");
        LayerMask formerMask = LayerMask.GetMask("Ceiling");

        Debug.Log("총 코너: " + myPath.corners.Length);

        for (int i = 0; i < myPath.corners.Length - 1; i++)
        {
            Vector3 dir = myPath.corners[i + 1] - myPath.corners[i];
            float dist = dir.magnitude;
            Debug.DrawLine(myPath.corners[i], myPath.corners[i + 1], Color.red);
            if (Physics.Raycast(myPath.corners[i], dir,
           out RaycastHit thit, dist + 1f, mask))
            {
                Debug.Log(LayerMask.LayerToName(thit.transform.gameObject.layer));
               
            }
           
        }
    }

    private void Awake()
    {
        myGamemanager = GameManagement.Inst;
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
        transform.position = myGamemanager.myMapManager.GetCrDestination(false).position;
    }
    // Start is called before the first frame update
    void Start()
    {
        myPath = new NavMeshPath();
        filter.areaMask += 1 << NavMesh.GetAreaFromName("Ceiling");
        filter.areaMask += 1 << NavMesh.GetAreaFromName("CrGround");
        filter.agentTypeID = GetAgentId("Crawler");
        ChangeState(STATE.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        StateProcess();
        myAnim.speed = myStat.MoveSpeed;
        Debug.DrawLine(drawings[0], drawings[1], Color.blue);
        DrawPath();
    }
    IEnumerator DelayState(STATE s, float time)
    {
        yield return new WaitForSeconds(time);
        ChangeState(s);
    }
    #region Mob Detect Sound
    public void HearingSound()
    {
        if (myState == STATE.Death || myState == STATE.Angry || myState == STATE.RagDoll ||
            myState == STATE.ResetBones || myState == STATE.StandUp) return;

        Transform tempTarget = myGamemanager.myPlayer.transform;
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
                    CheckSoundDist(myPath, 1 << NavMesh.GetAreaFromName("CrGround"), 
                        LayerMask.NameToLayer("Ground"), filter, () => LostTarget());
                }
            }
        }
        if (aiHeardPlayer)
        {
            ChangeState(STATE.Search);
        }
    }
    #endregion
    #region GetKickandRagDoll
    public override void GetKick(Vector3 dir, float strength)
    {
        if (myState == STATE.RagDoll) return;
        Vector3 force;
        Debug.Log("kick");
        ChangeState(STATE.RagDoll);
        force = dir * strength;
        force.y = strength;
        myRagDolls.myRagDoll.spineRigidBody.velocity += force * Time.fixedDeltaTime / (Time.timeScale * myRagDolls.myRagDoll.spineRigidBody.mass);
    }
    protected override void AlignRotationToHips()
    {
        Vector3 originHipPos = myHips.position;
        Quaternion originHipRot = myHips.rotation;

        Vector3 desireDir = myHips.up;
        desireDir.y = 0.0f;
        desireDir.Normalize();

        Quaternion fromtoRot = Quaternion.FromToRotation(transform.forward, desireDir);
        transform.rotation *= fromtoRot;

        myHips.position = originHipPos;
        myHips.rotation = originHipRot;
    }
    public override void ChangeRagDollState(RagDollState ragdoll)
    {
        switch (ragdoll)
        {
            case RagDollState.ResetBones:
                ChangeState(STATE.ResetBones);
                break;
            case RagDollState.StandUp:
                ChangeState(STATE.StandUp);
                break;
            case RagDollState.NoRagdoll:
                ChangeState(formerState);
                break;
        }
    }
    #endregion
    public void FindTarget(Transform target, STATE state)
    {
        if (myState == STATE.Death) return;
        if (target == myGamemanager.myPlayer.transform && IsGameOver) return;
        myTarget = target;
        StopAllCoroutines();
        ChangeState(state);
    }
    public void FindPlayer(Transform target)
    {
        FindTarget(target, STATE.Angry);
    }
    public void LostTarget()
    {
        if (myState == STATE.Death) return;
        myTarget = null;
        StopAllCoroutines();
        aiHeardPlayer = false;
        myAnim.SetBool("IsAngry", false);
        myAnim.SetBool("IsChasing", false);
        myAnim.SetBool("IsRunning", false);
        myAnim.SetBool("IsMoving", false); // 움직임 비활성화
        ChangeState(STATE.Idle);
    }
    public override void DeadMessage(Transform tr)
    {
        if (tr == myTarget)
        {
            IsGameOver = true;
            LostTarget();
        }
    }
    public Transform GetMyTarget()
    {
        return myTarget;
    }
    public NavMeshPath GetMyPath()
    {
        return myPath;
    }
    public Animator ReturnAnim()
    {
        return myAnim;
    }
    public int GetMobIndex()
    {
        return mobIndex;
    }
    public void SetMobIndex(int mobIndex)
    {
        this.mobIndex = mobIndex;
    }
    public void SetAnimTrigger()
    {
        myAnim.SetTrigger("Detect");
    }
    public AIState GetAIState()
    {
        if (myState == STATE.Angry)
        {
            return AIState.Angry;
        }
        return AIState.Normal;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if ((enemyMask & 1 << collision.gameObject.layer) != 0)
        {
            if (IsSearchable())
            {
                myAnim.SetTrigger("Detect");
                FindTarget(collision.transform, STATE.Angry);
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("NotGrabbableObj"))
        {
            if (collision.gameObject.TryGetComponent<Drawer>(out var obj))
            {
                if (!obj.IsDrawer)
                    obj.InteractwithMob();
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ceiling"))
        {
            if(myState == STATE.ToJump || myState == STATE.ToAir)
            {
                ChangeState(STATE.ToCeiling);
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (myState == STATE.ToJump)
            {
                ChangeState(STATE.ToAir);
            }
            if(myState == STATE.ToAir)
            {
                myRigid.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
            }
        }
    }
    public bool IsSearchable()
    {
        if (IsGameOver) return false;

        return (myState == STATE.Idle ||
            myState == STATE.Roaming ||
            myState == STATE.Search);
    }
}
