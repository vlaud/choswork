using System.Collections;
using System.Collections.Generic;
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
    public RagDollPhysics myRagDolls;
    public Transform myHips;
    public Transform myStart;
    public Transform myEnd;
    public LayerMask enemyMask = default;
    public bool IsStart = false;
    public float _timetoWakeup = 3.0f;
    public float _timeToResetBones;
    public float _changeStateTime = 3.0f;
    public string _standupName;
    public string _standupClipName;

    private NavMeshPath myPath;
    private Rigidbody rb;
    private CapsuleCollider cs;
    private float _origintimetoWake;
    private float _elapsedResetBonesTime;
    private BoneTransform[] _standupTransforms;
    private BoneTransform[] _ragdollTransforms;
    private Transform[] _bones;

    public enum STATE
    {
        Create, Idle, Roaming, Angry, Battle, RagDoll, StandUp, ResetBones, Death
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
                myAnim.SetBool("IsMoving", false); // 움직임 비활성화
                IsStart = Toggle.Inst.Toggling(IsStart);
                StartCoroutine(DelayState(STATE.Roaming));
                break;
            case STATE.Roaming:
                if (IsStart)
                {
                    RePath(myPath, myEnd.position, () => ChangeState(STATE.Idle));
                }
                else
                {
                    RePath(myPath, myStart.position, () => ChangeState(STATE.Idle));
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
        switch (myState)
        {
            case STATE.Create:
                break;
            case STATE.Idle:
                break;
            case STATE.Roaming:
                break;
            case STATE.Angry:
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
        rb = GetComponent<Rigidbody>();
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
        transform.position = myStart.position;
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
    IEnumerator DelayState(STATE s)
    {
        yield return new WaitForSeconds(_changeStateTime);
        ChangeState(s);
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
        rb.isKinematic = v;
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
