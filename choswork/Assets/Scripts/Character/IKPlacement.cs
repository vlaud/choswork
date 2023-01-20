using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class IKPlacement : MonoBehaviour
{
    private Animator myAnim;
    public LayerMask crashMask;
    [Range(0, 1f)]
    public float LDistanceToGround;
    [Range(0, 1f)]
    public float RDistanceToGround;
    [Range(0, 5f)]
    public float shootRayDist = 1f;
    [Range(0, 1f)]
    public float IKWeight = 1f;
    [Range(0, 1f)]
    public float curIKWeight;
    public float smoothMoveSpeed = 10.0f;
    private float Offset = 0.01f;
    
    public Vector3 leftRotOffset;
    public Vector3 leftRotation = new Vector3(60, 0, 0);

    public Vector3 rightRotOffset;
    public Vector3 rightRotation = new Vector3(60, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        myAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnAnimatorIK(int layerIndex)
    {
        IKWeight = (myAnim.GetBool("IsMoving")) ? 0f : 1f;
        curIKWeight = Mathf.Lerp(curIKWeight, IKWeight, Time.deltaTime * smoothMoveSpeed);

        curIKWeight = (1f - Offset < curIKWeight) ? 1f : (0f + Offset > curIKWeight) ? 0f : curIKWeight;
        
        if (myAnim)
        {
            myAnim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, curIKWeight);
            myAnim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, curIKWeight);
            myAnim.SetIKPositionWeight(AvatarIKGoal.RightFoot, curIKWeight);
            myAnim.SetIKRotationWeight(AvatarIKGoal.RightFoot, curIKWeight);

            // Left Foot
            if (Physics.Raycast(myAnim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down,
                out RaycastHit hit, LDistanceToGround + shootRayDist, crashMask))
            {
                Vector3 footPos = hit.point;
                Vector3 dir = footPos - myAnim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up;
                float dist = dir.magnitude;
                Debug.DrawRay(myAnim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down * dist, Color.green);
                footPos.y += LDistanceToGround;
                myAnim.SetIKPosition(AvatarIKGoal.LeftFoot, footPos);

                Quaternion rot = Quaternion.LookRotation(transform.forward, hit.normal);
                leftRotOffset.x = Mathf.Clamp(Vector3.Dot(transform.forward, hit.normal), -1f, 1f);
                leftRotOffset.z = Mathf.Clamp(Vector3.Dot(transform.right, hit.normal), -1f, 1f);
                Quaternion offset = Quaternion.Euler(new Vector3(
                    leftRotOffset.x * leftRotation.x, 
                    leftRotation.y, 
                    leftRotation.z * leftRotOffset.z));
                myAnim.SetIKRotation(AvatarIKGoal.LeftFoot, rot * offset);
            }
            // Right Foot
            if (Physics.Raycast(myAnim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down,
                out hit, RDistanceToGround + shootRayDist, crashMask))
            {
                Vector3 footPos = hit.point;
                Vector3 dir = footPos - myAnim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up;
                float dist = dir.magnitude;
                Debug.DrawRay(myAnim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down * dist, Color.green);
                footPos.y += RDistanceToGround;
                myAnim.SetIKPosition(AvatarIKGoal.RightFoot, footPos);

                Quaternion rot = Quaternion.LookRotation(transform.forward, hit.normal);
                rightRotOffset.x = Mathf.Clamp(Vector3.Dot(transform.forward, hit.normal), -1f, 1f);
                rightRotOffset.z = Mathf.Clamp(Vector3.Dot(transform.right, hit.normal), -1f, 1f);
                Quaternion offset = Quaternion.Euler(new Vector3(
                    rightRotOffset.x * rightRotation.x,
                    rightRotation.y, 
                    rightRotation.z * rightRotOffset.z));
                myAnim.SetIKRotation(AvatarIKGoal.RightFoot, rot * offset);
            }
        }
    }
}
