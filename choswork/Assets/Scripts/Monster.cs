using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public RagDollPhysics myRagDolls;
    public Player myEnemy;
    Rigidbody rb;
    CapsuleCollider cs;
    Vector3 force;
    // Start is called before the first frame update
    void Start()
    {
        myRagDolls.RagDollOnOff(false);
        rb = GetComponent<Rigidbody>();
        cs = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    
    public void GetKick()
    {
        Debug.Log("kick");
        rb.isKinematic = true;
        cs.isTrigger = true;
        force = myEnemy.myCameras.myRoot.forward * myEnemy.KickStrength;
        force.y = myEnemy.KickStrength;
        myRagDolls.RagDollOnOff(true);
        myRagDolls.myRagDoll.spineRigidBody.AddForce(force);
    }
}
