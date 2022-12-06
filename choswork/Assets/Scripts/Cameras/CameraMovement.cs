using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : CameraProperty
{
    public void CameraStart()
    {
        myCamset.myRig = transform;
        curRot.x = myCamset.myRig.localRotation.eulerAngles.x;
        curRot.y = myCamset.myRig.parent.localRotation.eulerAngles.y;
        camPos = myCamset.myCam.GetComponent<Transform>().localPosition;

        desireDistance = camPos.z;
    }
    public void GetCameraValue(CameraSet s)
    {
        curRot.x = myCamset.myRig.localRotation.eulerAngles.x;
        curRot.y = myCamset.myRig.parent.localRotation.eulerAngles.y;
        camPos = myCamset.myCam.GetComponent<Transform>().localPosition;

        desireDistance = camPos.z;
    }
    public virtual void CameraMoving()
    {

    }
    public void SpringArmWork()
    {
        myCamset.curRot.x -= Input.GetAxisRaw("Mouse Y") * LookupSpeed;
        myCamset.curRot.x = Mathf.Clamp(myCamset.curRot.x, LookupRange.x, LookupRange.y);

        myCamset.curRot.y += Input.GetAxisRaw("Mouse X") * LookupSpeed;
        myCamset.myRig.localRotation = Quaternion.Euler(myCamset.curRot.x, 0, 0);
        myCamset.myRig.parent.localRotation = Quaternion.Euler(0, myCamset.curRot.y, 0);

        
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, -camPos.z + Offset + 0.1f, crashMask))
        {
            camPos.z = -hit.distance + Offset;
        }
        else
        {
            camPos.z = Mathf.Lerp(camPos.z, desireDistance, Time.deltaTime * 3.0f);

        }
    }
}
