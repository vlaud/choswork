using System;
using System.Collections.Generic;
using System.Linq;

public static class AutoObserverRegistry
{
    private static Dictionary<Type, List<object>> listeners = new();

    /// <summary>
    /// 이벤트를 등록하는 함수
    /// </summary>
    /// <typeparam name="T">제너릭 타입</typeparam>
    /// <param name="listener">함수</param>
    public static void RegisterListener<T>(IListenTo<T> listener)
    {
        Type type = typeof(T);
        if (!listeners.ContainsKey(type))
            listeners[type] = new List<object>();

        if (!listeners[type].Contains(listener))
            listeners[type].Add(listener);
    }

    public static void UnregisterListener<T>(IListenTo<T> listener)
    {
        Type type = typeof(T);

        if (!listeners.ContainsKey(type)) return;

        List<object> list = listeners[type];

        // 역순으로 리스너 제거
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] == listener)
            {
                list.RemoveAt(i);

                if (list.Count == 0) listeners.Remove(type);

                return;
            }
        }
    }

    public static void Notify<T>(T evt)
    {
        if (listeners.TryGetValue(typeof(T), out var list))
        {
            foreach (var listener in list.OfType<IListenTo<T>>())
                listener.OnEventReceived(evt);
        }
    }
}
