public enum GameEventType
{
    Pause, 
    UnPause, 
}

public struct GameStatesEvent
{
    public GameEventType gameEventType;

    public GameStatesEvent(GameEventType gameEventType)
    {
        this.gameEventType = gameEventType;
    }

    static GameStatesEvent e;

    public static void Trigger(GameEventType gameEventType)
    {
        e.gameEventType = gameEventType;
        GameEventManager.TriggerEvent(e);
    }
}