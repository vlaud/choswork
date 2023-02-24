using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Monster : RagDollAction, AIAction
{ 
    private GameManagement myGamemanager;
    public LayerMask enemyMask = default;
    public Transform mobTarget; // for checking myTarget
    public bool IsGameOver = false;

    //mob startPos
    public bool IsStart = false;
    public int mobIndex;

    //ai hearing
    public Transform HearingTr; // 실제 추적 위치
    public Vector3 hearingPos;
    public Transform hearingObj; // 물건 위치
    private bool aiHeardPlayer = false;
    public float noiseTravelDistance = 10f;

    //ai path
    private NavMeshPath myPath;
    NavMeshQueryFilter filter;

    public enum STATE
    {
        Create, Idle, Roaming, Search, Angry, RagDoll, StandUp, ResetBones, Death
    }
    public STATE myState = STATE.Create;

    void ChangeState(STATE s)
    {
        if (myState == s) return;
        myState = s;
        
        switch (myState)
        {
            case STATE.Create:
                break;
            case STATE.Idle: // 평상시
                IsStart = !IsStart;
                myGamemanager.myMapManager.MobChangePath(IsStart, mobIndex);
                FindTarget(myGamemanager.myMapManager.GetDestination(IsStart, mobIndex), STATE.Idle);
                StartCoroutine(DelayState(STATE.Roaming, _changeStateTime));
                break;
            case STATE.Roaming:
                RePath(myPath, myTarget.position, filter, () => LostTarget());
                CorrectBaseHeight(myPath, myTarget, filter);
                break;
            case STATE.Angry:
                myAnim.SetBool("IsMoving", false); // 움직임 비활성화
                AttackTarget(myPath, myTarget, filter);
                break;
            case STATE.Search:
                myAnim.SetBool("IsMoving", false);
                myAnim.SetTrigger("Search");
                //RePath(myPath, myTarget.position, () => LostTarget(), "IsChasing");
                break;
            case STATE.RagDoll:
                StopAllCoroutines();
                RagDollSet(true);
                break;
            case STATE.StandUp:
                myAnim.Play(_standupName, -1, 0.0f);
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
                break;
            case STATE.Angry:
                myAnim.SetBool("IsAngry", true);
                break;
            case STATE.Search:
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
    private void Awake()
    {
        myGamemanager = GameManagement.Inst;
        cs = GetComponent<CapsuleCollider>();
        _origintimetoWake = _timetoWakeup;
        _bones = myHips.GetComponentsInChildren<Transform>();
        _standupTransforms = new BoneTransform[_bones.Length];
        _ragdollTransforms = new BoneTransform[_bones.Length];

        for (int boneIndex = 0; boneIndex < _bones.Length; ++boneIndex)
        {
            _standupTransforms[boneIndex] = new BoneTransform();
            _ragdollTransforms[boneIndex] = new BoneTransform();
        }
        PopulateAnimation(_standupClipName, _standupTransforms);
        RagDollSet(false);
        transform.position = myGamemanager.myMapManager.GetDestination(false, mobIndex).position;
    }
    // Start is called before the first frame update
    void Start()
    {
        myPath = new NavMeshPath();
        filter.areaMask = 1 << myGamemanager.myMapManager.surfaces.defaultArea;
        filter.agentTypeID = myGamemanager.myMapManager.surfaces.agentTypeID;
        ChangeState(STATE.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        StateProcess();
    }
    IEnumerator DelayState(STATE s, float time)
    {
        yield return new WaitForSeconds(time);
        ChangeState(s);
    }
    #region Mob Detect Sound
    void SetSoundPos()
    {
        var player = GameManagement.Inst.myPlayer.myHips;

        if (NavMesh.SamplePosition(hearingPos, out NavMeshHit hit, 10f, filter))
        {
            if (player.position.y < hit.position.y) // 물건이 천장으로 to ceiling
            {
                if (Physics.Raycast(hearingPos, Vector3.down, out RaycastHit thit,
                    20f, 1 << LayerMask.NameToLayer("Ground")))
                {
                    Debug.Log("천장" + thit.point);
                    hearingPos = thit.point;
                }
            }
            else  // 물건이 바닥으로 to floor
            {
                Debug.Log("바닥" + hit.position);
                hearingPos = hit.position;
            }
        }
        HearingTr.position = hearingPos;
    }
    void CheckSoundDist()
    {
        SetSoundPos();
        float dist = Vector3.Distance(hearingPos, transform.position);
        if(noiseTravelDistance >= dist)
        {
            Debug.Log("몹이 소리를 들었다.");
            Debug.Log("듣는 위치: " + hearingPos);
            Debug.Log("거리: " + dist);
            aiHeardPlayer = true;
            myTarget = HearingTr;
            RePath(myPath, myTarget.position, filter, () => LostTarget(), "IsChasing");
        }
        else
        {
            Debug.Log("못 들었다.");
            aiHeardPlayer = false;
        }
    }
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
                    CheckSoundDist();
                    //grab.IsSoundable = false;
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
    public override void ChangeRagDollState(RagDollState ragdoll) 
    {
        switch(ragdoll)
        {
            case RagDollState.ResetBones:
                ChangeState(STATE.ResetBones);
                break;
            case RagDollState.StandUp:
                ChangeState(STATE.StandUp);
                break;
            case RagDollState.NoRagdoll:
                FindTarget(myGamemanager.myPlayer.transform, STATE.Angry);
                break;
        }
    }
    #endregion
    public void AttackCheck(bool v)
    {
        //myAnim.GetComponent<RootMotion>().DontRot = v;
    }
    public void FindTarget(Transform target, STATE state)
    {
        if (myState == STATE.Death) return;
        if(target == myGamemanager.myPlayer.transform && IsGameOver) return;
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
    public override Animator ReturnAnim()
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
        if(myState == STATE.Angry)
        {
            return AIState.Angry;
        }
        return AIState.Normal;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if((enemyMask & 1 << collision.gameObject.layer) != 0)
        {
            if (IsSearchable())
            {
                myAnim.SetTrigger("Detect");
                FindTarget(collision.transform, STATE.Angry);
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("NotGrabbableObj"))
        {
            if(collision.gameObject.TryGetComponent<Drawer>(out var obj))
            {
                if(!obj.IsDrawer)
                    obj.InteractwithMob();
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
