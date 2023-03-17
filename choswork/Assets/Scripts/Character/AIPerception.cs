using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Normal, Angry
}
public interface AIAction
{
    AIState GetAIState();
    void FindPlayer(Transform target);
    void LostTarget();
    bool IsSearchable();
    NavMeshPath GetMyPath();
    Transform GetMyTarget();
    void SetAnimTrigger();
    void HearingSound();
    int GetMobIndex();
    void SetMobIndex(int mobIndex);
}
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
    public LayerMask enemyMask = default;
    public LayerMask obstructionMask = default;
    public Transform myTarget;
    private GameManagement myGamemanager;
    private AIAction myMonster;
    private WaitForSeconds delayCoroutine = new WaitForSeconds(0.2f);

    void ChangeState(State s)
    {
        if (myState == s) return;
        myState = s;

        switch (myState)
        {
            case State.Search:
                StopAllCoroutines();
                myGamemanager.myMainmenu?.PlayInGameMusic();
                StartCoroutine(FOVRoutine());
                break;
            case State.Chase:
                StopAllCoroutines();
                myGamemanager.myMainmenu?.PlayDangerMusic();
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
            else if (myMonster.GetAIState() == AIState.Angry)
                ChangeState(State.Chase);
            yield return delayCoroutine;
        }
    }
    IEnumerator CheckDist()
    {
        while (myState == State.Chase)
        {
            if(myMonster.GetAIState() == AIState.Angry)
            {
                if (CalcPathLength(myMonster.GetMyPath(), myMonster.GetMyTarget().position) > lostDist)
                {
                    myMonster?.LostTarget();
                    //LostTarget?.Invoke();
                    myTarget = null;
                    ChangeState(State.Search);
                }
            }
            yield return delayCoroutine;
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
                    {
                        canSeePlayer = true;
                        myTarget = target;
                        //foundPlayer?.Invoke(myTarget, Monster.STATE.Angry);
                        myMonster?.FindPlayer(myTarget);
                        //myMonster.ReturnAnim().SetTrigger("Detect");
                        myMonster?.SetAnimTrigger();
                        Debug.Log("플레이어 발견!");
                    }
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
        myMonster = transform.GetComponent<AIAction>();
        ChangeState(State.Search);
    }

    // Update is called once per frame
    void Update()
    {
        StateProcess();
    }
}
