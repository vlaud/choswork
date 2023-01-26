using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public class Monster : BattleSystem
{
    private class BoneTransform
    {
        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
    }
    public LayerMask enemyMask = default;

    //mob ragdoll
    public RagDollPhysics myRagDolls;
    public Transform myHips;
    public float _timetoWakeup = 3.0f;
    public float _timeToResetBones;
    public float _changeStateTime = 3.0f;
    public string _standupName;
    public string _standupClipName;
    private CapsuleCollider cs;
    private float _origintimetoWake;
    private float _elapsedResetBonesTime;
    private BoneTransform[] _standupTransforms;
    private BoneTransform[] _ragdollTransforms;
    private Transform[] _bones;

    //mob startPos
    public bool IsStart = false;

    //ai sight
    public bool playerIsInLOS = false;
    public float fovAngle = 160f;
    public float losRadius = 45f;
    public float lostDist = 10f;

    //ai sight and memory
    private bool aiMemorizesPlayer = false;
    public float memoryStartTime = 10f;
    private float increasingMemoryTime;

    //ai hearing
    public Vector3 hearingPos;
    public Transform hearingObj;
    private bool aiHeardPlayer = false;
    public float noiseTravelDistance = 10f;

    //ai path
    private NavMeshPath myPath;
   

    public enum STATE
    {
        Create, Idle, Roaming, Angry, Search, Battle, RagDoll, StandUp, ResetBones, Death
    }
    public STATE myState = STATE.Create;

    void ChangeState(STATE s)
    {
        var manager = GameManagement.Inst.myMapManager;
        if (myState == s) return;
        myState = s;
        
        switch (myState)
        {
            case STATE.Create:
                break;
            case STATE.Idle: // 평상시
                StopAllCoroutines();
                aiHeardPlayer = false;
                myAnim.SetBool("IsAngry", false);
                myAnim.SetBool("IsChasing", false);
                myAnim.SetBool("IsRunning", false);
                myAnim.SetBool("IsMoving", false); // 움직임 비활성화
                IsStart = !IsStart;
                manager.MobChangePath(IsStart);
                StartCoroutine(DelayState(STATE.Roaming, _changeStateTime));
                break;
            case STATE.Roaming:
                if (IsStart)
                {
                    RePath(myPath, manager.EndPoint.position, () => ChangeState(STATE.Idle));
                }
                else
                {
                    RePath(myPath, manager.StartPoint.position, () => ChangeState(STATE.Idle));
                }
                break;
            case STATE.Angry:
                myAnim.SetBool("IsMoving", false); // 움직임 비활성화
                if (myTarget == null)
                {
                    myTarget = GameObject.Find("Player").GetComponent<Transform>();
                }
                AttackTarget(myPath, myTarget);
                break;
            case STATE.Search:
                StopAllCoroutines();
                myAnim.SetBool("IsMoving", false);
                myAnim.SetTrigger("Search");
                RePath(myPath, hearingPos, () => ChangeState(STATE.Idle), "IsChasing");
                break;
            case STATE.Battle:
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
        var manager = GameManagement.Inst.myMapManager;
        switch (myState)
        {
            case STATE.Create:
                break;
            case STATE.Idle:
                HearingSound();
                break;
            case STATE.Roaming:
                HearingSound();
                break;
            case STATE.Angry:
                if(CalcPathLength(myPath, myTarget.position) > lostDist)
                {
                    ChangeState(STATE.Idle);
                }    
                break;
            case STATE.Search:
                break;
            case STATE.Battle:
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
        //myHips = myAnim.GetBoneTransform(HumanBodyBones.Hips);
        cs = GetComponent<CapsuleCollider>();
        _origintimetoWake = _timetoWakeup;
        //_bones = myRagDolls.myRagDollsTransforms;
        _bones = myHips.GetComponentsInChildren<Transform>();
        _standupTransforms = new BoneTransform[_bones.Length];
        _ragdollTransforms = new BoneTransform[_bones.Length];

        for (int boneIndex = 0; boneIndex < _bones.Length; ++boneIndex)
        {
            _standupTransforms[boneIndex] = new BoneTransform();
            _ragdollTransforms[boneIndex] = new BoneTransform();
            //Debug.Log(_bones[boneIndex]);
        }
        PopulateAnimation(_standupClipName, _standupTransforms);
        RagDollSet(false);
        transform.position = GameManagement.Inst.myMapManager.StartPoint.position;
    }
    // Start is called before the first frame update
    void Start()
    {
        myPath = new NavMeshPath();
        //RePath(myPath, "IsMoving", myEnd.position);
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
    void CheckSoundDist()
    {
        float dist = Vector3.Distance(hearingPos, transform.position);
        if(noiseTravelDistance >= dist)
        {
            Debug.Log("몹이 소리를 들었다.");
            aiHeardPlayer = true;
        }
        else
        {
            Debug.Log("못 들었다.");
            aiHeardPlayer = false;
        }
        Debug.Log("거리: " + dist);
    }
    void HearingSound()
    {
        if(aiHeardPlayer)
        {
            ChangeState(STATE.Search);
        }
        Transform tempTarget = GameObject.Find("Player").GetComponent<Transform>();
        if ((tempTarget != null && tempTarget.TryGetComponent<PlayerPickUpDrop>(out var target)))
        {
            if (target.GetObjectGrabbable() != null)
            {
                hearingObj = target.GetObjectGrabbable().transform;
            }
            else if (hearingObj != null && hearingObj.TryGetComponent<ObjectGrabbable>(out var grab))
            {
                if (grab.IsSoundable)
                {
                    hearingPos = grab.soundPos;
                    CheckSoundDist();
                    Debug.Log("듣는 위치: " + hearingPos);
                    grab.IsSoundable = false;
                }
            }
        }
    }
    public void GetKick(Vector3 dir, float strength)
    {
        if (myState == STATE.RagDoll) return;
        Vector3 force;
        Debug.Log("kick");
        ChangeState(STATE.RagDoll);
        force = dir * strength;
        force.y = strength;
        myRagDolls.myRagDoll.spineRigidBody.AddForce(force);
        myAnim.SetBool("IsAngry", true);
    }

    public void RagDollSet(bool v)
    {
        myRigid.isKinematic = v;
        cs.isTrigger = v;
        myAnim.enabled = !v;
        myRagDolls.RagDollOnOff(v);
    }
    void AlignRotationToHips()
    {
        Vector3 originHipPos = myHips.position;
        Quaternion originHipRot = myHips.rotation;

        Vector3 desireDir = myHips.up * -1.0f;
        desireDir.y = 0.0f;
        desireDir.Normalize();

        Quaternion fromtoRot = Quaternion.FromToRotation(transform.forward, desireDir);
        transform.rotation *= fromtoRot;

        myHips.position = originHipPos;
        myHips.rotation = originHipRot;
    }
    void AlignPositonToHips()
    {
        Vector3 originHipPos = myHips.position;
        transform.position = myHips.position;
        Vector3 positionOffset = _standupTransforms[0].Position;
        positionOffset.y = 0.0f;
        positionOffset = transform.rotation * positionOffset;
        transform.position -= positionOffset;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
        myHips.position = originHipPos;
    }
    void RagdollBehaviour()
    {
        _timetoWakeup -= Time.deltaTime;

        if (_timetoWakeup <= 0.0f)
        {
            AlignRotationToHips();
            AlignPositonToHips();

            PopulateBoneTransforms(_ragdollTransforms);
            ChangeState(STATE.ResetBones);
            _elapsedResetBonesTime = 0.0f;
        }
    }
    void StandingUpBehaviour()
    {
        if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName(_standupName))
        {
            ChangeState(STATE.Angry);
        }
    }
    void ResetBonesBehaviour()
    {
        _elapsedResetBonesTime += Time.deltaTime;
        float elapsedPercentage = _elapsedResetBonesTime / _timeToResetBones;

        for (int i = 0; i < _bones.Length; ++i)
        {
            _bones[i].localPosition = Vector3.Lerp(
                _ragdollTransforms[i].Position,
                _standupTransforms[i].Position,
                elapsedPercentage);

            _bones[i].localRotation = Quaternion.Lerp(
                  _ragdollTransforms[i].Rotation,
                    _standupTransforms[i].Rotation,
                    elapsedPercentage);
        }
        if (elapsedPercentage >= 1.0f)
        {
            ChangeState(STATE.StandUp);
            RagDollSet(false);
            _timetoWakeup = _origintimetoWake;
        }
    }
    void PopulateBoneTransforms(BoneTransform[] bonetransforms)
    {
        for (int boneIndex = 0; boneIndex < _bones.Length; ++boneIndex)
        {
            bonetransforms[boneIndex].Position = _bones[boneIndex].localPosition;
            bonetransforms[boneIndex].Rotation = _bones[boneIndex].localRotation;
        }
    }
    void PopulateAnimation(string clipName, BoneTransform[] bonetransforms)
    {
        Vector3 positionBeforeSampling = transform.position;
        Quaternion rotationBeforeSampling = transform.rotation;
        foreach (AnimationClip clip in myAnim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                clip.SampleAnimation(myAnim.gameObject, 0);
                PopulateBoneTransforms(bonetransforms);
                break;
            }
        }
        transform.position = positionBeforeSampling;
        transform.rotation = rotationBeforeSampling;
    }
    public void Attack()
    {

    }
    public void AttackCheck(bool v)
    {
        //myAnim.GetComponent<RootMotion>().DontRot = v;
    }
}
