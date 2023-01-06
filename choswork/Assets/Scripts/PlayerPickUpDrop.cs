using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private Player myPlayer;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private CameraSet? curCamset;
    float pickUpDistance = 2f;
    private void Awake()
    {
        curCamset = myPlayer.myCameras.GetMyCamera();
        playerCameraTransform = curCamset?.myRig;
        objectGrabPointTransform = curCamset?.GrabPoint;
    }
    private void Update()
    {
        CheckItem();
    }
    void CheckItem()
    {
        curCamset = myPlayer.myCameras.GetMyCamera();
        playerCameraTransform = curCamset?.myRig;
        objectGrabPointTransform = curCamset?.GrabPoint;
        if (playerCameraTransform == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
                out RaycastHit hit, pickUpDistance, pickUpLayerMask))
            {
                if(hit.transform.TryGetComponent(out ObjectGrabbable objectGrabbable))
                {
                    Debug.Log(objectGrabbable);
                    objectGrabbable.Grab(objectGrabPointTransform);
                }
            }
        }
        
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * pickUpDistance, Color.blue);
    }
}
