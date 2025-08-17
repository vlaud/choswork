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

    public void RagDollOnOff(bool v)
    {
        isRagdoll = v;
        Rigidbody[] rigidbodies = GetAllRigidbodies();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = !v;
            rb.GetComponent<Collider>().enabled = v;
            
            if (v)
            {
                // Ragdoll 활성화 시 설정
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                
                // // 속도 제한 설정
                // rb.maxLinearVelocity = 10f;
                // rb.maxAngularVelocity = 10f;
                
                // // 마찰력과 감쇠 설정
                // rb.linearDamping = 0.5f;
                // rb.angularDamping = 0.5f;
                
                // // CharacterJoint가 있다면 충돌 활성화
                // CharacterJoint joint = rb.GetComponent<CharacterJoint>();
                // if (joint != null)
                // {
                //     joint.enableCollision = true;
                //     joint.enablePreprocessing = true;
                    
                //     // 관절 제한 설정
                //     SoftJointLimit limit = new SoftJointLimit();
                //     limit.limit = 30f;
                //     limit.bounciness = 0f;
                //     limit.contactDistance = 0f;
                    
                //     joint.lowTwistLimit = limit;
                //     joint.highTwistLimit = limit;
                //     joint.swing1Limit = limit;
                //     joint.swing2Limit = limit;
                // }
            }
        }
    }
}
