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
    [SerializeField] private float Strength = 10000.0f;
    [SerializeField] private float pickUpDistance = 2f;

    private ObjectGrabbable objectGrabbable;
    private ObjectGrabbable showObject;
    private void Awake()
    {
        curCamset = myPlayer.myCameras.GetMyCamera();
        playerCameraTransform = curCamset?.myRig;
    }
    private void Update()
    {
        CheckItem();
    }
    void CheckItem()
    {
        curCamset = myPlayer.myCameras.GetMyCamera();
        playerCameraTransform = curCamset?.myRig;
        if (playerCameraTransform == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            ThrowObject();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUpAndDrop();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            objectGrabbable?.GetComponent<PickUpController>()?.CanPickUp();
        }
        ShowItemUI();
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * pickUpDistance, Color.blue);
    }
    void ShowItemUI()
    {
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
            out RaycastHit hit, pickUpDistance, pickUpLayerMask))
        {
            if (objectGrabbable == null)
            {
                showObject = hit.transform.GetComponent<ObjectGrabbable>();
                showObject?.SetText();
                showObject?.SetItemInfoAppear(true);
            }
        }
        else
        {
            showObject?.SetItemInfoAppear(false);
        }
    }
    void PickUpAndDrop()
    {
        if (objectGrabbable == null)
        {
            // 물체 X
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
            out RaycastHit hit, pickUpDistance, pickUpLayerMask))
            {
                if (hit.transform.TryGetComponent(out objectGrabbable))
                {
                    objectGrabbable.Grab(objectGrabPointTransform);
                    objectGrabbable.SetItemInfoAppear(false);
                    Debug.Log(objectGrabbable);
                }
            }
        }
        else
        {
            // 물체 O
            objectGrabbable.Drop();
            objectGrabbable = null;
        }
    }
    void ThrowObject()
    {
        if (objectGrabbable != null)
        {
            objectGrabbable.Throw(objectGrabPointTransform.forward, Strength);
            objectGrabbable = null;
        }
    }
}
