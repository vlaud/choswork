using System;
using System.Collections.Generic;

#region Event_Listener_Interface
public interface EventListenerBase { }

public interface EventListener<T> : EventListenerBase where T : struct
{
    void OnEvent(T eventType);
}

public interface iSubscription
{
    void Subscribe();
    void Unsubscribe();
}

#endregion

// 클래스 이름을 EventBus로 변경하고, 게임 이벤트 관리자 역할을 명시
#region EventBus
/// <summary>
/// 게임 이벤트 버스 클래스
/// </summary>
public static class EventBus
{
    // 이벤트 타입별로 구독자(리스너) 목록을 관리하는 사전
    private static readonly Dictionary<Type, List<EventListenerBase>> _subscribersList
        = new Dictionary<Type, List<EventListenerBase>>();

    /// <summary>
    /// 이벤트를 구독하는 메서드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="listener"></param>
    // 제네릭 제약조건을 IGameEvent로 변경
    public static void Subscribe<T>(EventListener<T> listener) where T : struct
    {
        Type eventType = typeof(T);

        if (!_subscribersList.ContainsKey(eventType))
        {
            _subscribersList[eventType] = new List<EventListenerBase>();
        }

        if (!SubscriptionExists(eventType, listener))
        {
            _subscribersList[eventType].Add(listener);
        }
    }

    /// <summary>
    /// 이벤트 구독 취소 메서드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="listener"></param>
    // 제네릭 제약조건을 IGameEvent로 변경
    public static void Unsubscribe<T>(EventListener<T> listener) where T : struct
    {
        Type eventType = typeof(T);

        if (!_subscribersList.ContainsKey(eventType))
            return;

        List<EventListenerBase> subscriberList = _subscribersList[eventType];

        for (int i = subscriberList.Count - 1; i >= 0; i--)
        {
            if (subscriberList[i] == listener)
            {
                subscriberList.RemoveAt(i);

                if (subscriberList.Count == 0)
                {
                    _subscribersList.Remove(eventType);
                }
                return;
            }
        }
    }

    /// <summary>
    /// 이미 구독 중인지 확인하는 메서드
    /// </summary>
    /// <param name="type"></param>
    /// <param name="receiver"></param>
    /// <returns></returns>
    private static bool SubscriptionExists(Type type, EventListenerBase receiver)
    {
        List<EventListenerBase> receivers;
        if (!_subscribersList.TryGetValue(type, out receivers)) return false;

        bool exists = false;
        for (int i = receivers.Count - 1; i >= 0; i--)
        {
            if (receivers[i] == receiver)
            {
                exists = true;
                Debug.Log("already exists: " + receiver);
                break;
            }
        }
        return exists;
    }

    /// <summary>
    /// 이벤트를 발생시키는 메서드 (이름을 Trigger로 변경)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="events"></param>
    // 제네릭 제약조건을 IGameEvent로 변경
    public static void Trigger<T>(T events) where T : struct
    {
        Type eventType = typeof(T);
        List<EventListenerBase> list;
        if (!_subscribersList.TryGetValue(eventType, out list))
        {
            return;
        }

        for (int i = list.Count - 1; i >= 0; i--)
        {
            (list[i] as EventListener<T>).OnEvent(events);
        }
    }
}
#endregion

// 이벤트 리스너를 등록 및 해제하는 확장 메서드

#region GameEventsRegister
/// <summary>
/// 이벤트 리스너를 등록 및 해제하는 확장 메서드
/// </summary>
public static class GameEventsRegister
{
    public delegate void Delegate<T>(T eventType);

    /// <summary>
    /// 이벤트 리스너를 등록하는 확장 메서드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="caller"></param>
    // 제네릭 제약조건을 IGameEvent로 변경
    public static void EventStartingListening<T>(this EventListener<T> caller) where T : struct
    {
        EventBus.Subscribe(caller); // EventBus 클래스 이름 변경에 맞춰 수정
    }

    /// <summary>
    /// 이벤트 리스너를 해제하는 확장 메서드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="caller"></param>
    // 제네릭 제약조건을 IGameEvent로 변경
    public static void EventStopListening<T>(this EventListener<T> caller) where T : struct
    {
        EventBus.Unsubscribe(caller); // EventBus 클래스 이름 변경에 맞춰 수정
    }
}
#endregion