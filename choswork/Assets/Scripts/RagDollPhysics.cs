using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct RagDoll
{
    public Rigidbody hipsRigidBody;
    public Rigidbody leftUpLegRigidBody;
    public Rigidbody leftLegRigidBody;
    public Rigidbody rightUpLegRigidBody;
    public Rigidbody rightLegRigidBody;
    public Rigidbody spineRigidBody;
    public Rigidbody leftArmRigidBody;
    public Rigidbody leftForeArmRigidBody;
    public Rigidbody rightArmRigidBody;
    public Rigidbody rightForeArmRigidBody;
    public Rigidbody headRigidBody;
}
public class RagDollPhysics : MonoBehaviour
{
    public RagDoll myRagDoll;
    public bool OnOff = false;
    public void RagDollOnOff(bool v) //래그돌 온오프 함수
    {
        myRagDoll.headRigidBody.gameObject.SetActive(v);
        myRagDoll.hipsRigidBody.gameObject.SetActive(v);
        myRagDoll.leftUpLegRigidBody.gameObject.SetActive(v);
        myRagDoll.leftLegRigidBody.gameObject.SetActive(v);
        myRagDoll.rightUpLegRigidBody.gameObject.SetActive(v);
        myRagDoll.rightLegRigidBody.gameObject.SetActive(v);
        myRagDoll.spineRigidBody.gameObject.SetActive(v);
        myRagDoll.leftArmRigidBody.gameObject.SetActive(v);
        myRagDoll.leftForeArmRigidBody.gameObject.SetActive(v);
        myRagDoll.rightArmRigidBody.gameObject.SetActive(v);
        myRagDoll.rightForeArmRigidBody.gameObject.SetActive(v);
    }
}
