using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    [SerializeField] private float dropSpeed;
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;
    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }
    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.useGravity = false;
        objectRigidbody.isKinematic = true;
        
    }
    public void Drop()
    {
        Throw(Vector3.down, dropSpeed);
    }
    public void Throw(Vector3 dir, float strength)
    {
        this.objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;
        objectRigidbody.isKinematic = false;
        Vector3 force;
        Debug.Log(dir);
        force = dir * strength;
        //objectRigidbody.AddForce(force);
        objectRigidbody.velocity = force;
    }
    private void FixedUpdate()
    {
        if(objectGrabPointTransform != null)
        {
            float speed = 10f;
            Vector3 newPos = Vector3.Lerp(transform.position, objectGrabPointTransform.position, speed * Time.deltaTime);
            objectRigidbody.MovePosition(newPos);
        }
        if (objectRigidbody.useGravity) objectRigidbody.AddForce(Physics.gravity * (objectRigidbody.mass * objectRigidbody.mass));
    }
}
