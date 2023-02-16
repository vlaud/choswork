using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : RagDollAction
{
    private GameManagement myGamemanager;
    public enum STATE
    {
        Create, Idle, Roaming, Search, Angry, RagDoll, StandUp, ResetBones, Death
    }
    public STATE myState = STATE.Create;
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
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
