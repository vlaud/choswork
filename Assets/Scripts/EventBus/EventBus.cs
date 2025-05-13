using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Event_Listener_Interface
public interface EventListenerBase { }

public interface EventListener<T> : EventListenerBase
{
    void OnEvent(T eventType);
}

public interface iSubscription
{
    void Subscribe();
    void Unsubscribe();
}

#endregion

// 게임 이벤트 관리자 클래스
#region GameEventManager
/// <summary>
/// 게임 이벤트 관리자 클래스
/// </summary>
public class GameEventManager
{
    // 이벤트 타입별로 구독자(리스너) 목록을 관리하는 사전
    private static readonly Dictionary<Type, List<EventListenerBase>> _subscribersList
        = new Dictionary<Type, List<EventListenerBase>>();

    /// <summary>
    /// 이벤트를 구독하는 메서드
    /// </summary>
    /// <typeparam name="GameEvents"></typeparam>
    /// <param name="listener"></param>
    public static void Subscribe<GameEvents>(EventListener<GameEvents> listener) where GameEvents : struct
    {
        Type eventType = typeof(GameEvents);

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
    /// <typeparam name="GameEvents"></typeparam>
    /// <param name="listener"></param>
    public static void Unsubscribe<GameEvents>(EventListener<GameEvents> listener) where GameEvents : struct
    {
        Type eventType = typeof(GameEvents);

        if (!_subscribersList.ContainsKey(eventType))
            return;

        List<EventListenerBase> subscriberList = _subscribersList[eventType];

        for (int i = subscriberList.Count - 1; i >= 0; i--)
        {
            if (subscriberList[i] == listener)
            {
                subscriberList.Remove(subscriberList[i]);

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
    /// 이벤트를 발생시키는 메서드
    /// </summary>
    /// <typeparam name="GameEvents"></typeparam>
    /// <param name="events"></param>
    public static void TriggerEvent<GameEvents>(GameEvents events) where GameEvents : struct
    {
        Type eventType = typeof(GameEvents);
        List<EventListenerBase> list;
        if (!_subscribersList.TryGetValue(eventType, out list))
        {
            return;
        }

        for (int i = list.Count - 1; i >= 0; i--)
        {
            (list[i] as EventListener<GameEvents>).OnEvent(events);
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
    /// <typeparam name="EventType"></typeparam>
    /// <param name="caller"></param>
    public static void EventStartingListening<EventType>(this EventListener<EventType> caller) where EventType : struct
    {
        GameEventManager.Subscribe(caller);
    }

    /// <summary>
    /// 이벤트 리스너를 해제하는 확장 메서드
    /// </summary>
    /// <typeparam name="EventType"></typeparam>
    /// <param name="caller"></param>
    public static void EventStopListening<EventType>(this EventListener<EventType> caller) where EventType : struct
    {
        GameEventManager.Unsubscribe(caller);
    }
}
#endregion