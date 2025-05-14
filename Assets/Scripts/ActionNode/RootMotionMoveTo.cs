using TheKiwiCoder;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class RootMotionMoveTo : ActionNode
{
    public bool isKeepRunning = false;
    public bool RotationEnabled = false;
    public string DesiredAction;

    protected override void OnStart() {
        context.characterMovement.RePath(blackboard.movement.myPath, 
            blackboard.movement.GetMyTarget(), blackboard.movement.filter, () => DesiringMethod());
    }

    protected override void OnStop() {
        blackboard.movement.StopAllCoroutines();
    }

    protected override State OnUpdate() {
        if (RotationEnabled) blackboard.movement.RotateToTarget(blackboard.Target.position);

        if (isKeepRunning)
        {
            if (RotationEnabled)
            {
                if(CheckRotation(blackboard.Target))
                    return State.Success;

                return State.Running;
            }
            return State.Success;
        }

        if (blackboard.movement.state != MovementState.Idle)
        {
            return State.Running;
        }

        if (blackboard.movement.GetMyTarget() == null)
        {
            return State.Success;
        }

        if (blackboard.movement.myPath.status == NavMeshPathStatus.PathInvalid)
        {
            return State.Failure;
        }

        return State.Running;
    }

    void DesiringMethod()
    {
        if (DesiredAction == "")
            return;

        blackboard.movement.StartCoroutine(DesiredAction);
    }

    bool CheckRotation(Transform target)
    {
        Vector3 dir = (target.position - context.transform.position).normalized;
        dir.y = 0.0f;
        float Angle = Vector3.Angle(context.transform.forward, dir);
        
        if (Angle > Mathf.Epsilon)
        {
            return false;
        }
        return true;
    }

}
