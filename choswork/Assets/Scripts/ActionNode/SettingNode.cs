[System.Serializable]
public class SettingNode : GenericTypeActionNode<IMovement>
{
    protected override void OnStart() {
        value = context.gameObject.GetComponent<IMovement>();
        blackboard.movement = value;
        blackboard.Target = value.target;
        blackboard.aiState = value.aiState;
        blackboard.rdState = value.rdState;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if(blackboard.movement == null) 
            return State.Success;
        return State.Failure;
    }
}
