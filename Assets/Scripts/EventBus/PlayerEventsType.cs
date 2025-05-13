public enum PlayerEventType
{
    TimeStop, 
    PlayerKick, 
    Dash, 
    NormalSpeed,
}

public struct PlayerStatesEvent
{
    public PlayerEventType playerEventType;

    public PlayerStatesEvent(PlayerEventType playerEventType)
    {
        this.playerEventType = playerEventType;
    }

    static PlayerStatesEvent e;

    public static void Trigger(PlayerEventType playerEventType)
    {
        e.playerEventType = playerEventType;
        GameEventManager.TriggerEvent(e);  // GameEventManager 클래스를 사용하여 이벤트를 트리거합니다.
    }
}