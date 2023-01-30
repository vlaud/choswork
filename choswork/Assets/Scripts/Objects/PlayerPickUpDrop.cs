using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDrop : InputManager
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private Player myPlayer;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private CameraSet? curCamset;
    [SerializeField] private float Strength = 20.0f;
    [SerializeField] private float pickUpDistance = 6f;
    [SerializeField] private Projection _projection;
    private ObjectGrabbable objectGrabbable;
    private Door DoorInteractable;
    private ObjectInteractable showObject;
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

        // Handle object pickup and throw
        HandleObjectPickupAndThrow();
        ShowItemUI();
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * pickUpDistance, Color.blue);

        if (objectGrabbable != null)
        {
            _projection.SimulateTrajectory(objectGrabbable, objectGrabPointTransform.position,
           objectGrabPointTransform.forward, Strength);
        }
        else transform.GetComponent<LineRenderer>().positionCount = 0;
    }
    public override void GetItem()
    {
        if (objectGrabbable?.GetComponent<PickUpController>() == null) return;
        objectGrabbable?.GetComponent<PickUpController>()?.CanPickUp();
        objectGrabbable = null;
    }
    void ShowItemUI()
    {
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
            out RaycastHit hit, pickUpDistance, pickUpLayerMask))
        {
            if (objectGrabbable == null)
            {
                showObject = hit.transform.GetComponent<ObjectInteractable>();
                showObject?.SetText();
                showObject?.SetItemInfoAppear(true);
            }
        }
        else
        {
            showObject?.SetItemInfoAppear(false);
        }
    }
    public override void InteractObj()
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
                else if(hit.transform.TryGetComponent(out DoorInteractable))
                {
                    DoorInteractable.InteractDoor();
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
    public override void ThrowObject()
    {
        if (objectGrabbable != null)
        {
            objectGrabbable.Throw(objectGrabPointTransform.forward, Strength);
            objectGrabbable = null;
        }
    }
    public ObjectGrabbable GetObjectGrabbable()
    {
        return objectGrabbable;
    }
}
