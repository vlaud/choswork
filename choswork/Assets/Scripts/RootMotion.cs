using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotion : MonoBehaviour
{
    public bool DontMove = false;
    public Transform myPlayer;
    Vector3 deltaPosition = Vector3.zero;
    Quaternion deltaRotation = Quaternion.identity;
    
    
    private void FixedUpdate()
    {
        if (DontMove) return;
        myPlayer.Translate(deltaPosition, Space.World);
        deltaPosition = Vector3.zero;
        myPlayer.rotation *= deltaRotation;
        deltaRotation = Quaternion.identity;
    }
    private void OnAnimatorMove()
    {
        deltaPosition += GetComponent<Animator>().deltaPosition;
        deltaRotation *= GetComponent<Animator>().deltaRotation;
    }
}
