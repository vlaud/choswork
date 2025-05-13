public enum UIEventType
{
    Enable,
    Disable,
    LeftClick,
    RightClick,
}

public struct UIStatesEvent
{
    public UIEventType uIEventType;

    public UIStatesEvent(UIEventType uIEventType)
    {
        this.uIEventType = uIEventType;
    }

    static UIStatesEvent e;

    public static void Trigger(UIEventType uIEventType)
    {
        e.uIEventType = uIEventType;
        GameEventManager.TriggerEvent(e);
    }
}