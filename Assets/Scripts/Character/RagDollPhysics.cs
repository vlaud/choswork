using System;
using System.Reflection;
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
    public bool isRagdoll;

    public Rigidbody[] GetAllRigidbodies()
    {
        FieldInfo[] fields = typeof(RagDoll).GetFields();
        Rigidbody[] rigidbodies = new Rigidbody[fields.Length];

        for (int i = 0; i < fields.Length; i++)
        {
            rigidbodies[i] = (Rigidbody)fields[i].GetValue(myRagDoll);
        }

        return rigidbodies;
    }

    public void RagDollOnOff(bool v) //래그돌 온오프 함수
    {
        isRagdoll = v;
        myRagDoll.headRigidBody.isKinematic = !v;
        myRagDoll.headRigidBody.GetComponent<Collider>().enabled = v;
        myRagDoll.hipsRigidBody.isKinematic = !v;
        myRagDoll.hipsRigidBody.GetComponent<Collider>().enabled = v;
        myRagDoll.leftUpLegRigidBody.isKinematic = !v;
        myRagDoll.leftUpLegRigidBody.GetComponent<Collider>().enabled = v;
        myRagDoll.leftLegRigidBody.isKinematic = !v;
        myRagDoll.leftLegRigidBody.GetComponent<Collider>().enabled = v;
        myRagDoll.rightUpLegRigidBody.isKinematic = !v;
        myRagDoll.rightUpLegRigidBody.GetComponent<Collider>().enabled = v;
        myRagDoll.rightLegRigidBody.isKinematic = !v;
        myRagDoll.rightLegRigidBody.GetComponent<Collider>().enabled = v;
        myRagDoll.spineRigidBody.isKinematic = !v;
        myRagDoll.spineRigidBody.GetComponent<Collider>().enabled = v;
        myRagDoll.leftArmRigidBody.isKinematic = !v;
        myRagDoll.leftArmRigidBody.GetComponent<Collider>().enabled = v;
        myRagDoll.leftForeArmRigidBody.isKinematic = !v;
        myRagDoll.leftForeArmRigidBody.GetComponent<Collider>().enabled = v;
        myRagDoll.rightArmRigidBody.isKinematic = !v;
        myRagDoll.rightArmRigidBody.GetComponent<Collider>().enabled = v;
        myRagDoll.rightForeArmRigidBody.isKinematic = !v;
        myRagDoll.rightForeArmRigidBody.GetComponent<Collider>().enabled = v;
    }
}
