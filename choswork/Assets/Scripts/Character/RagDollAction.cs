using UnityEngine;

public enum RagDollState
{
    ResetBones, StandUp, NoRagdoll
}
public class RagDollAction : AIDetectionMovement
{
    protected class BoneTransform
    {
        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
    }
    protected CapsuleCollider cs;
    public RagDollPhysics myRagDolls;
    public float _timetoWakeup = 3.0f;
    public float _timeToResetBones;
    public float _changeStateTime = 3.0f;
    protected float _origintimetoWake;
    protected float _elapsedResetBonesTime;
    public Transform myHips;
    protected Transform[] _bones;
    public string _faceUpStandUpStateName;
    public string _faceDownStandUpStateName;
    public string _faceUpStandUpClipName;
    public string _faceDownStandUpClipName;
    protected BoneTransform[] _faceUpStandUpBoneTransforms;
    protected BoneTransform[] _faceDownStandUpBoneTransforms;
    protected BoneTransform[] _ragdollTransforms;
    private bool _isFacingUp;
    public virtual void GetKick(Vector3 dir, float strength) {}

    public void RagDollSet(bool v)
    {
        myRigid.isKinematic = v;
        cs.isTrigger = v;
        myAnim.enabled = !v;
        myRagDolls.RagDollOnOff(v);
    }
    protected virtual void AlignRotationToHips()
    {
        Vector3 originHipPos = myHips.position;
        Quaternion originHipRot = myHips.rotation;

        Vector3 desireDir = myHips.up;

        if(_isFacingUp)
        {
            desireDir *= -1;
        }

        desireDir.y = 0.0f;
        desireDir.Normalize();

        Quaternion fromtoRot = Quaternion.FromToRotation(transform.forward, desireDir);
        transform.rotation *= fromtoRot;

        myHips.position = originHipPos;
        myHips.rotation = originHipRot;
    }
    protected void AlignPositonToHips()
    {
        Vector3 originHipPos = myHips.position;
        transform.position = myHips.position;
        Vector3 positionOffset = GetStandUpBoneTransforms()[0].Position;
        positionOffset.y = 0.0f;
        positionOffset = transform.rotation * positionOffset;
        transform.position -= positionOffset;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
        myHips.position = originHipPos;
    }
    public void RagdollBehaviour()
    {
        _timetoWakeup -= Time.deltaTime;

        if (_timetoWakeup <= 0.0f)
        {
            _isFacingUp = myHips.forward.y > 0;

            AlignRotationToHips();
            AlignPositonToHips();

            PopulateBoneTransforms(_ragdollTransforms);
            ChangeRagDollState(RagDollState.ResetBones);
            _elapsedResetBonesTime = 0.0f;
        }
    }
    protected void StandingUpBehaviour()
    {
        if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName(GetStandUpStateName()))
        {
            ChangeRagDollState(RagDollState.NoRagdoll);
        }
    }
    public void ResetBonesBehaviour()
    {
        _elapsedResetBonesTime += Time.deltaTime;
        float elapsedPercentage = _elapsedResetBonesTime / _timeToResetBones;

        BoneTransform[] standUpBoneTransform = GetStandUpBoneTransforms();

        for (int i = 0; i < _bones.Length; ++i)
        {
            _bones[i].localPosition = Vector3.Lerp(
                _ragdollTransforms[i].Position,
                standUpBoneTransform[i].Position,
                elapsedPercentage);

            _bones[i].localRotation = Quaternion.Lerp(
                  _ragdollTransforms[i].Rotation,
                    standUpBoneTransform[i].Rotation,
                    elapsedPercentage);
        }
        if (elapsedPercentage >= 1.0f)
        {
            ChangeRagDollState(RagDollState.StandUp);
            RagDollSet(false);
            _timetoWakeup = _origintimetoWake;
        }
    }
    protected void PopulateBoneTransforms(BoneTransform[] bonetransforms)
    {
        for (int boneIndex = 0; boneIndex < _bones.Length; ++boneIndex)
        {
            bonetransforms[boneIndex].Position = _bones[boneIndex].localPosition;
            bonetransforms[boneIndex].Rotation = _bones[boneIndex].localRotation;
        }
    }
    protected void PopulateAnimation(string clipName, BoneTransform[] bonetransforms)
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

    protected string GetStandUpStateName()
    {
        return _isFacingUp ? _faceUpStandUpStateName : _faceDownStandUpStateName;
    }

    private BoneTransform[] GetStandUpBoneTransforms()
    {
        return _isFacingUp ? _faceUpStandUpBoneTransforms : _faceDownStandUpBoneTransforms;
    }

    public virtual void ChangeRagDollState(RagDollState ragdoll) {}
}
