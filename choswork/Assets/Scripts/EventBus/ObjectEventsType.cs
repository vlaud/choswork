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
        GameEventManager.TriggerEvent(e);  // GameEventManager 클래스를 사용하여 이벤트를 트리거합니다.
    }
}