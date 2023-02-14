using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ObjectGrabbable : ObjectInteractable
{
    [SerializeField] private float dropSpeed;
    [SerializeField] private LayerMask objectMask = default;
    public Vector3 soundPos;
    public bool IsSoundable;
    public float _changeStateTime = 0.5f;
    public List<UnityAction> hearings = new List<UnityAction>();
    private Rigidbody objectRigidbody;
    private Collider objectCollider;
    private Transform objectGrabPointTransform;
    private bool _isGhost;
    public enum STATE
    {
        Create, Idle, Grab, Throw, WallHit
    }
    public STATE myState = STATE.Create;
    void ChangeState(STATE s)
    {
        if (myState == s) return;
        myState = s;

        switch (myState)
        {
            case STATE.Create:
                break;
            case STATE.Idle: // 평상시
                IsSoundable = false;
                break;
            case STATE.Grab:
                foreach (var hear in hearings) hear.Invoke();
                break;
            case STATE.Throw:
                break;
            case STATE.WallHit:
                soundPos = transform.position;
                IsSoundable = true;
                foreach (var hear in hearings) hear.Invoke();
                Debug.Log("물건 위치: " + soundPos);
                StartCoroutine(DelayState(STATE.Idle));
                break;
        }
    }
    void StateProcess()
    {
        switch (myState)
        {
            case STATE.Create:
                break;
            case STATE.Idle:
                break;
            case STATE.Grab:
                break;
            case STATE.Throw:
                break;
            case STATE.WallHit:
                break;
        }
    }
    IEnumerator DelayState(STATE s)
    {
        yield return new WaitForSeconds(_changeStateTime);
        ChangeState(s);
    }
    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();
        SetText();
        SetItemInfoAppear(false);
        ChangeState(STATE.Idle);
    }
    private void Start()
    {
        for (int i = 0; i < GameManagement.Inst.myMonsters.Length; ++i)
             hearings.Add(GameManagement.Inst.myMonsters[i].HearingSound);
    }
    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        SetObjectPhysics(false);
        ChangeState(STATE.Grab);
    }
    public void Drop()
    {
        Throw(Vector3.down, dropSpeed);
    }
    public void Throw(Vector3 dir, float strength, bool isGhost = false)
    {
        _isGhost = isGhost;
        this.objectGrabPointTransform = null;
        SetObjectPhysics(true);
        Vector3 force;
        force = dir * strength;
        //objectRigidbody.AddForce(force);
        //objectRigidbody.velocity = force;
        objectRigidbody.velocity += force * Time.fixedDeltaTime / (Time.timeScale * objectRigidbody.mass);
        ChangeState(STATE.Throw);
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
        if (_isGhost) return;
        if (myState != STATE.Throw) return;
        if ((objectMask & 1 << collision.gameObject.layer) != 0)
        {
            //오브젝트에 부딪칠 때
            ChangeState(STATE.WallHit);
        }
    }
}
