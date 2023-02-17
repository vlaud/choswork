using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Crawler : RagDollAction
{
    private GameManagement myGamemanager;
    public LayerMask enemyMask = default;
    public Transform mobTarget; // for checking myTarget
    public bool IsGameOver = false;

    //mob startPos
    public bool IsStart = false;

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
                myGamemanager.myMapManager.CrawlerChangePath(IsStart);
                FindTarget(myGamemanager.myMapManager.GetCrDestination(IsStart), STATE.Idle);
                StartCoroutine(DelayState(STATE.Roaming, _changeStateTime));
                break;
            case STATE.Roaming:
                RePath(myPath, myTarget.position, filter, () => LostTarget());
                break;
            case STATE.Angry:
                myAnim.SetBool("IsMoving", false); // 움직임 비활성화
                AttackTarget(myPath, myTarget, filter);
                break;
            case STATE.Search:
                myAnim.SetBool("IsMoving", false);
                myAnim.SetTrigger("Search");
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
        transform.position = myGamemanager.myMapManager.GetCrDestination(false).position;
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
    Vector3 GetNextCorner(NavMeshPath myPath)
    {
        Vector3[] list = myPath.corners;
        return list[1];
    }
    // Start is called before the first frame update
    void Start()
    {
        myPath = new NavMeshPath();
        filter.areaMask = 1 << NavMesh.GetAreaFromName("Ceiling");
        filter.agentTypeID = GetAgentId("Crawler");
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
    public void FindTarget(Transform target, STATE state)
    {
        if (myState == STATE.Death) return;
        if (target == myGamemanager.myPlayer.transform && IsGameOver) return;
        myTarget = target;
        StopAllCoroutines();
        ChangeState(state);
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
    public STATE GetMyState()
    {
        return myState;
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
    }
    public bool IsSearchable()
    {
        if (IsGameOver) return false;

        return (myState == STATE.Idle ||
            myState == STATE.Roaming ||
            myState == STATE.Search);
    }
}
