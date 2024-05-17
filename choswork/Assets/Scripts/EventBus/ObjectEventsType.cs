public enum ObjectEventType
{
    ThrowObject, 
    InteractObj, 
    GetItem,
}

public struct ObjectStatesEvent
{
    public ObjectEventType objectEventType;

    public ObjectStatesEvent(ObjectEventType objectEventType)
    {
        this.objectEventType = objectEventType;
    }

    static ObjectStatesEvent e;

    public static void Trigger(ObjectEventType objectEventType)
    {
        e.objectEventType = objectEventType;
        GameEventManager.TriggerEvent(e);  // GameEventManager Ŭ������ ����Ͽ� �̺�Ʈ�� Ʈ�����մϴ�.
    }
}