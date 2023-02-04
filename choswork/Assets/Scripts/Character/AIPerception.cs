using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using static AIPerception;
using static UnityEngine.GraphicsBuffer;

public class AIPerception : MonoBehaviour
{
    public enum State
    {
        Create, Search, Chase
    }
    public State myState = State.Create;

    //ai sight
    public bool canSeePlayer = false;
    [Range(0, 360)]
    public float fovAngle = 160f;
    public float losRadius = 5f;
    public float lostDist = 10f;
    public UnityAction<Transform, Monster.STATE> foundPlayer;
    public UnityAction LostTarget;
    public LayerMask enemyMask = default;
    public LayerMask obstructionMask = default;
    public Transform myTarget;
    private GameManagement myGamemanager;
    private Monster myMonster;

    void ChangeState(State s)
    {
        if (myState == s) return;
        myState = s;

        switch (myState)
        {
            case State.Search:
                StopAllCoroutines();
                StartCoroutine(FOVRoutine());
                break;
            case State.Chase:
                StopAllCoroutines();
                StartCoroutine(CheckDist());
                break;
        }
    }
    void StateProcess()
    {
        switch (myState)
        {
            case State.Search:
                break;
            case State.Chase:
                break;
        }
    }
    IEnumerator FOVRoutine()
    {
        while (myState == State.Search)
        {
            if(myMonster.IsSearchable())
                FieldOfViewCheck();
            else if (myMonster.GetMyState() == Monster.STATE.Angry)
                ChangeState(State.Chase);
            yield return new WaitForSeconds(0.2f);
        }
    }
    IEnumerator CheckDist()
    {
        while (myState == State.Chase)
        {
            if(myMonster.GetMyState() == Monster.STATE.Angry)
            {
                if (CalcPathLength(myMonster.GetMyPath(), myMonster.GetMyTarget().position) > lostDist)
                {
                    LostTarget?.Invoke();
                    myTarget = null;
                    ChangeState(State.Search);
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, losRadius, enemyMask);
        Transform target;
        if (rangeChecks.Length != 0)
        {
            target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            
            if (Vector3.Angle(transform.forward, directionToTarget) < fovAngle * 0.5f)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    myTarget = target;
                    myMonster.ReturnAnim().SetTrigger("Detect");
                    foundPlayer?.Invoke(myTarget, Monster.STATE.Angry);
                    Debug.Log("플레이어 발견!");
                }
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
        }
        /*
        float dist = lostDist;
        Transform target = null;
        foreach (Collider col in rangeChecks)
        {
            float tempDist = Vector3.Distance(col.transform.position, transform.position);
            if (dist > tempDist)
            {
                dist = tempDist;
                target = col.GetComponent<Transform>();
            }
        }
        */
    }
    float CalcPathLength(NavMeshPath myPath, Vector3 _targetPos)
    {
        NavMesh.CalculatePath(transform.position, _targetPos, NavMesh.AllAreas, myPath);

        Vector3[] _wayPoint = new Vector3[myPath.corners.Length + 2];

        _wayPoint[0] = transform.position;
        _wayPoint[myPath.corners.Length + 1] = _targetPos;

        float _pathLength = 0;  // 경로 길이를 더함
        for (int i = 0; i < myPath.corners.Length; i++)
        {
            _wayPoint[i + 1] = myPath.corners[i];
            _pathLength += Vector3.Distance(_wayPoint[i], _wayPoint[i + 1]);
        }

        return _pathLength;
    }
    // Start is called before the first frame update
    void Start()
    {
        myGamemanager = GameManagement.Inst;
        myMonster = myGamemanager.myMonster;
        foundPlayer = myMonster.FindTarget;
        LostTarget = myMonster.LostTarget;
        ChangeState(State.Search);
    }

    // Update is called once per frame
    void Update()
    {
        StateProcess();
    }
}
