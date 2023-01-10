using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class ObjectGrabbable : MonoBehaviour
{
    [SerializeField] private float dropSpeed;
    [SerializeField] private TMPro.TMP_Text actionText;  // 행동을 보여 줄 텍스트
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;
    public string PickUpMsg = " 집기 ";
    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        SetText();
        SetItemInfoAppear(false);
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
    public void SetItemInfoAppear(bool v) //오브젝트와 아이템 집기 UI 보이기
    {
        actionText.gameObject.SetActive(v);
    }
    public void SetText()
    {
        if (actionText == null) return;
        actionText.text = transform.GetComponent<ItemPickUp>()?.item.itemName + PickUpMsg + "<color=yellow>" + "(E)" + "</color>";
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
