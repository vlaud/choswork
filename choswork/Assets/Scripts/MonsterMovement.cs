using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : Movement
{
    public override void LostTarget()
    {
        myTarget = null;
        aiState = AIState.Normal;
        ChangeState(MovementState.Idle);
        Debug.Log("Å¸°Ù ³õÄ§");
    }
    public override void GetKick(Vector3 dir, float strength)
    {
        if (myRagDolls.isRagdoll) return;
        Vector3 force;
        Debug.Log("kick");
        StopAllCoroutines();
        RagDollSet(true);
        force = dir * strength;
        force.y = strength;
        myRagDolls.myRagDoll.spineRigidBody.velocity += force * Time.fixedDeltaTime / (Time.timeScale * myRagDolls.myRagDoll.spineRigidBody.mass);
    }
}
