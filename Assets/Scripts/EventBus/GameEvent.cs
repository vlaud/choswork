using System;

/// <summary>
/// 데이터(페이로드)가 없는 제네릭 이벤트 구조체
/// </summary>
/// <typeparam name="T">이벤트 종류를 정의한 Enum 타입</typeparam>
public struct GameEvent<T> where T : Enum
{
    public T Type { get; }
    public GameEvent(T type) => Type = type;


    public static void Trigger(T type)
    {
        var e = new GameEvent<T>(type);
        EventBus.Trigger(e);
    }
}

/// <summary>
/// 데이터(페이로드)를 포함하는 제네릭 이벤트 구조체
/// </summary>
/// <typeparam name="T">이벤트 종류를 정의한 Enum 타입</typeparam>
/// <typeparam name="TPayload">전달할 데이터의 타입</typeparam>
public struct GameEvent<T, TPayload> where T : Enum
{
    public T Type { get; }
    public TPayload Payload { get; }

    public GameEvent(T type, TPayload payload)
    {
        Type = type;
        Payload = payload;
    }

    public static void Trigger(T type, TPayload payload)
    {
        var e = new GameEvent<T, TPayload>(type, payload);
        EventBus.Trigger(e);
    }
}
