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

// ���� �̺�Ʈ ������ Ŭ����
#region GameEventManager
/// <summary>
/// ���� �̺�Ʈ ������ Ŭ����
/// </summary>
public class GameEventManager
{
    // �̺�Ʈ Ÿ�Ժ��� ������(������) ����� �����ϴ� ����
    private static readonly Dictionary<Type, List<EventListenerBase>> _subscribersList
        = new Dictionary<Type, List<EventListenerBase>>();

    /// <summary>
    /// �̺�Ʈ�� �����ϴ� �޼���
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
    /// �̺�Ʈ ���� ��� �޼���
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
    /// �̹� ���� ������ Ȯ���ϴ� �޼���
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
    /// �̺�Ʈ�� �߻���Ű�� �޼���
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

// �̺�Ʈ �����ʸ� ��� �� �����ϴ� Ȯ�� �޼���

#region GameEventsRegister
/// <summary>
/// �̺�Ʈ �����ʸ� ��� �� �����ϴ� Ȯ�� �޼���
/// </summary>
public static class GameEventsRegister
{
    public delegate void Delegate<T>(T eventType);

    /// <summary>
    /// �̺�Ʈ �����ʸ� ����ϴ� Ȯ�� �޼���
    /// </summary>
    /// <typeparam name="EventType"></typeparam>
    /// <param name="caller"></param>
    public static void EventStartingListening<EventType>(this EventListener<EventType> caller) where EventType : struct
    {
        GameEventManager.Subscribe(caller);
    }

    /// <summary>
    /// �̺�Ʈ �����ʸ� �����ϴ� Ȯ�� �޼���
    /// </summary>
    /// <typeparam name="EventType"></typeparam>
    /// <param name="caller"></param>
    public static void EventStopListening<EventType>(this EventListener<EventType> caller) where EventType : struct
    {
        GameEventManager.Unsubscribe(caller);
    }
}
#endregion