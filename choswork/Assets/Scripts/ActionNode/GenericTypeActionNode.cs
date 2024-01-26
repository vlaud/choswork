using TheKiwiCoder;

[System.Serializable]
public class GenericTypeActionNode<T> : ActionNode where T : class
{
    public T value;
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
