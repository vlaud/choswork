using TheKiwiCoder;
using UnityEngine;

[System.Serializable]
public class CheckDistance : ActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        // 목표물이 없으면 실패를 반환한다.
        if (blackboard.movement.GetMyTarget() == null)
        {
            return State.Failure;
        }

        // 공격 범위를 가져온다.
        var attackRange = blackboard.movement.myStat.AttackRange;

        // 목표물과의 거리 제곱을 계산한다.
        var sqrDist = (blackboard.movement.GetMyTarget().position - context.transform.position).sqrMagnitude;

        // 거리 제곱과 공격 범위 제곱을 비교하여 성공 또는 실패를 반환한다.
        if (sqrDist <= attackRange * attackRange)
        {
            return State.Success;
        }

        return State.Failure;
    }
}