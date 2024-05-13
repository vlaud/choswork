public enum CameraEventType
{
    FPS, 
    TPS, 
    UI,
    NotUI,
    Debug,
}

public struct CameraStatesEvent
{
    public CameraEventType cameraEventType;

    public CameraStatesEvent(CameraEventType cameraEventType)
    {
        this.cameraEventType = cameraEventType;
    }

    static CameraStatesEvent e;

    public static void Trigger(CameraEventType cameraEventType)
    {
        e.cameraEventType = cameraEventType;
        GameEventManager.TriggerEvent(e);
    }
}