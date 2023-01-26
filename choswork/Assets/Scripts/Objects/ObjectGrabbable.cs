using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ObjectGrabbable : ObjectInteractable
{
    [SerializeField] private float dropSpeed;
    [SerializeField] private bool IsSoundable;
    [SerializeField] private LayerMask objectMask = default;
    public Vector3 soundPos;
    private Rigidbody objectRigidbody;
    private Collider objectCollider;
    private Transform objectGrabPointTransform;
    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();
        SetText();
        SetItemInfoAppear(false);
    }
    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        SetObjectPhysics(false);
    }
    public void Drop()
    {
        Throw(Vector3.down, dropSpeed);
    }
    public void Throw(Vector3 dir, float strength)
    {
        this.objectGrabPointTransform = null;
        SetObjectPhysics(true);
        Vector3 force;
        Debug.Log(dir);
        force = dir * strength;
        //objectRigidbody.AddForce(force);
        objectRigidbody.velocity = force;
        IsSoundable = true;
    }
    public void SetObjectPhysics(bool v)
    {
        objectRigidbody.useGravity = v;
        objectRigidbody.isKinematic = !v;
        objectCollider.isTrigger = !v;
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
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsSoundable) return;
        if ((objectMask & 1 << collision.gameObject.layer) != 0)
        {
            //¿ÀºêÁ§Æ®¿¡ ºÎµúÄ¥ ¶§
            soundPos = transform.position;
            IsSoundable = false;
        }
    }
}
