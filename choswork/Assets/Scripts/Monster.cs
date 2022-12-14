using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public RagDollPhysics myRagDolls;
    // Start is called before the first frame update
    void Start()
    {
        myRagDolls.RagDollOnOff(false);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    
    public void GetKick()
    {
        Debug.Log("kick");
        myRagDolls.RagDollOnOff(true);
        myRagDolls.myRagDoll.spineRigidBody.AddForce(new Vector3(0f, 10000f, 10000f));
    }
}
