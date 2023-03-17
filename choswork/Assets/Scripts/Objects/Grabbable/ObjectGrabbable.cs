using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
            case STATE.Idle: // ����
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
                Debug.Log("���� ��ġ: " + soundPos);
                StartCoroutine(DelayState(STATE.Idle));
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
             hearings.Add(GameManagement.Inst.myMonsters[i].GetComponent<AIAction>().HearingSound);
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
        FreeObj(isGhost);
        Vector3 force;
        force = dir * strength;
        objectRigidbody.velocity += force * Time.fixedDeltaTime / (Time.timeScale * objectRigidbody.mass);
        ChangeState(STATE.Throw);
    }
    public void ReleaseObj(bool isGhost = false, STATE state = STATE.Idle)
    {
        FreeObj(isGhost);
        ChangeState(state);
    }
    void FreeObj(bool isGhost = false)
    {
        _isGhost = isGhost;
        this.objectGrabPointTransform = null;
        SetObjectPhysics(true);
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
            //������Ʈ�� �ε�ĥ ��
            ChangeState(STATE.WallHit);
        }
    }
}
