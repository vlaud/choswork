using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AIDetectionMovement : BattleSystem
{
    //ai hearing
    public Transform HearingTr; // ���� ���� ��ġ
    public Vector3 hearingPos;
    public Transform hearingObj; // ���� ��ġ
    public bool aiHeardPlayer = false;
    public float noiseTravelDistance = 10f;

    #region Mob Detect Sound
    protected void SetSoundPos(int navlayer, int Physicsmask)
    {
        var player = GameManagement.Inst.myPlayer.myHips;

        if (NavMesh.SamplePosition(hearingPos, out NavMeshHit hit, 10f, navlayer))
        {
            if (player.position.y < hit.position.y) // ������ õ������ to ceiling
            {
                if (Physics.Raycast(hearingPos, Vector3.down, out RaycastHit thit,
                    20f, 1 << Physicsmask))
                {
                    Debug.Log("õ��" + thit.point);
                    hearingPos = thit.point;
                }
            }
            else  // ������ �ٴ����� to floor
            {
                Debug.Log("�ٴ�" + hit.position);
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
                Debug.Log("�Ҹ��� ������� �ʹ� �ִ�.");
                aiHeardPlayer = false;
            }
            else
            {
                myTarget = HearingTr;
                RePath(myPath, myTarget, filter, done, anim);
                Debug.Log("���� �Ҹ��� �����.");
                Debug.Log("��� ��ġ: " + hearingPos);
                Debug.Log("�Ÿ�: " + dist);
                aiHeardPlayer = true;
            }
        }
        else
        {
            Debug.Log("�� �����.");
            aiHeardPlayer = false;
        }
    }
    protected bool TrackSoundFailed(NavMeshPath myPath)
    {
        if (myPath.status != NavMeshPathStatus.PathComplete)
        {
            Debug.Log("��� ����: " + transform);
            Debug.Log(transform + "'s status: " + myPath.status);
            return true;
        }
        return false;
    }
    #endregion
}
