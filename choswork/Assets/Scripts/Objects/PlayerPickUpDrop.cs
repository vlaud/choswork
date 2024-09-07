using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour, iSubscription, EventListener<ObjectStatesEvent>
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private Player myPlayer;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private LayerMask obstructionMask;
    [SerializeField] private CameraSet? curCamset;
    [SerializeField] private float Strength = 20.0f;
    [SerializeField] private float pickUpDistance = 6f;
    [SerializeField] private Projection _projection;
    private ObjectGrabbable objectGrabbable;
    private ObjectNotGrabbable Interactable;
    private ObjectInteractable showObject;
    private void Awake()
    {
        curCamset = myPlayer.myCameras.GetMyCamera();
        playerCameraTransform = curCamset?.myRig;
    }

    private void Start()
    {
        Subscribe();
    }

    private void Update()
    {
        CheckItem();
        ShowItemUI();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void OnEvent(ObjectStatesEvent eventType)
    {
        switch (eventType.objectEventType)
        {
            case ObjectEventType.ThrowObject:
                ThrowObject();
                break;
            case ObjectEventType.InteractObj:
                InteractObj();
                break;
            case ObjectEventType.GetItem:
                GetItem();
                break;
        }
    }

    public void Subscribe()
    {
        this.EventStartingListening();
    }

    public void Unsubscribe()
    {
        this.EventStopListening();
    }

    private void CheckItem()
    {
        if (objectGrabbable != null)
        {
            _projection.SetSimulation(objectGrabbable, objectGrabPointTransform.position,
           objectGrabPointTransform.forward, Strength);
        }
    }

    private void SetObjectToNullAction()
    {
        objectGrabbable = null;
        _projection.StopSimultation();
        transform.GetComponent<LineRenderer>().positionCount = 0;
    }

    public void GetItem()
    {
        if (objectGrabbable?.GetComponent<PickUpController>() == null) return;
        objectGrabbable?.GetComponent<PickUpController>()?.CanPickUp();
        SetObjectToNullAction();
    }

    void ShowItemUI()
    {
        curCamset = myPlayer.myCameras.GetMyCamera();
        playerCameraTransform = curCamset?.myRig;
        if (playerCameraTransform == null) return;

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
            out RaycastHit hit, pickUpDistance, obstructionMask | pickUpLayerMask))
        {
            float dist = Vector3.Distance(hit.point, playerCameraTransform.position);
            Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * dist, Color.blue);
            if ((obstructionMask & 1 << hit.transform.gameObject.layer) != 0)
            {
                showObject?.SetItemInfoAppear(false);
                return;
            }
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

    public void InteractObj()
    {
        if (objectGrabbable == null)
        {
            // 물체 X
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
            out RaycastHit hit, pickUpDistance, pickUpLayerMask | obstructionMask))
            {
                if ((obstructionMask & 1 << hit.transform.gameObject.layer) != 0) return;

                if (hit.transform.TryGetComponent(out objectGrabbable))
                {
                    objectGrabbable.Grab(objectGrabPointTransform);
                    objectGrabbable.SetItemInfoAppear(false);
                    Debug.Log(objectGrabbable);
                }
                else if (hit.transform.TryGetComponent(out Interactable))
                {
                    Interactable.Interact();
                }
            }
        }
        else
        {
            // 물체 O
            objectGrabbable.Drop();
            SetObjectToNullAction();
        }
    }

    public void ThrowObject()
    {
        if (objectGrabbable != null)
        {
            objectGrabbable.Throw(objectGrabPointTransform.forward, Strength);
            SetObjectToNullAction();
        }
    }

    public ObjectGrabbable GetObjectGrabbable()
    {
        return objectGrabbable;
    }

    public Vector3 GetobjectGrabPointForward()
    {
        return objectGrabPointTransform.forward;
    }
}
