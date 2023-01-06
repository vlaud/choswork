using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Player myPlayer;
    [SerializeField] private LayerMask pickUpLayerMask;
    float pickUpDistance = 2f;
    private void Awake()
    {
        playerCameraTransform = myPlayer.myCameras.GetMyCamera();
    }
    private void Update()
    {
        CheckItem();
    }
    void CheckItem()
    {
        playerCameraTransform = myPlayer.myCameras.GetMyCamera();
        if (playerCameraTransform == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
                out RaycastHit hit, pickUpDistance, pickUpLayerMask))
            {
                Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * pickUpDistance, Color.red);
                Debug.Log(hit.transform);
            }
        }
    }
}
