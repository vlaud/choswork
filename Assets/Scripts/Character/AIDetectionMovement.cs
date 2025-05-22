using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AIDetectionMovement : BattleSystem
{
    //ai hearing
    public Transform HearingTr; // 실제 추적 위치
    public Vector3 hearingPos;
    public Transform hearingObj; // 물건 위치
    public bool aiHeardPlayer = false;
    public float noiseTravelDistance = 10f;

    #region Mob Detect Sound
    protected void SetSoundPos(int navlayer, int Physicsmask)
    {
        var player = GameManagement.Inst.myPlayer.myHips;

        if (NavMesh.SamplePosition(hearingPos, out NavMeshHit hit, 10f, navlayer))
        {
            if (player.position.y < hit.position.y) // 물건이 천장으로 to ceiling
            {
                if (Physics.Raycast(hearingPos, Vector3.down, out RaycastHit thit,
                    20f, 1 << Physicsmask))
                {
                    Debug.Log("천장" + thit.point);
                    hearingPos = thit.point;
                }
            }
            else  // 물건이 바닥으로 to floor
            {
                Debug.Log("바닥" + hit.position);
                hearingPos = hit.position;
            }
        }
        HearingTr.position = hearingPos;
    }
    protected void CheckSoundDist(NavMeshPath myPath, int navlayer, int Physicsmask, NavMeshQueryFilter filter, UnityAction done = null, string anim = "IsChasing")
    {
        SetSoundPos(navlayer, Physicsmask);
        float dist = Vector3.Distance(hearingPos, transform.position);
        if (noiseTravelDistance >= dist)
        {
            NavMesh.CalculatePath(transform.position, HearingTr.position, filter, myPath);
            if(myPath.status != NavMeshPathStatus.PathComplete)
            {
                Debug.Log(transform + "'s status: " + myPath.status);
                Debug.Log("소리를 들었으나 너무 멀다.");
                aiHeardPlayer = false;
            }
            else
            {
                myTarget = HearingTr;
                RePath(myPath, myTarget, filter, done, anim);
                Debug.Log("몹이 소리를 들었다.");
                Debug.Log("듣는 위치: " + hearingPos);
                Debug.Log("거리: " + dist);
                aiHeardPlayer = true;
            }
        }
        else
        {
            Debug.Log("못 들었다.");
            aiHeardPlayer = false;
        }
    }
    protected bool TrackSoundFailed(NavMeshPath myPath)
    {
        if (myPath.status != NavMeshPathStatus.PathComplete)
        {
            Debug.Log("경로 실패: " + transform);
            Debug.Log(transform + "'s status: " + myPath.status);
            return true;
        }
        return false;
    }

    #endregion
}
