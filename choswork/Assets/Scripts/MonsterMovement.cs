using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : Movement
{
    public override void LostTarget()
    {
        aiState = AIState.Normal;
        Debug.Log("Å¸°Ù ³õÄ§");
        target = null;
    }
    public override void GetKick(Vector3 dir, float strength)
    {
        Vector3 force;
        Debug.Log("kick");
        force = dir * strength;
        force.y = strength;
        myRagDolls.myRagDoll.spineRigidBody.velocity += force * Time.fixedDeltaTime / (Time.timeScale * myRagDolls.myRagDoll.spineRigidBody.mass);
    }
}
