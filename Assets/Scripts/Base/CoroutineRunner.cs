using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 코루틴을 돌리는 클래스
/// </summary>
public static class CoroutineRunner
{
    public static void StopCurrentCoroutine(this MonoBehaviour mono, ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            mono.StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public static void StartOrRestartCoroutine(this MonoBehaviour mono, ref Coroutine coroutine, IEnumerator coAction)
    {
        StopCurrentCoroutine(mono, ref coroutine);
        coroutine = mono.StartCoroutine(coAction);
    }

    public static void StartManagedCoroutine<TKey>(this MonoBehaviour mono, ref Dictionary<TKey, Coroutine> dict, TKey key, IEnumerator routine)
    {
        if (dict.TryGetValue(key, out Coroutine existing))
        {
            StopCurrentCoroutine(mono, ref existing);
        }

        StartOrRestartCoroutine(mono, ref existing, routine);
        dict[key] = existing;
    }

    public static void StopManagedCoroutine<TKey>(this MonoBehaviour mono, ref Dictionary<TKey, Coroutine> dict, TKey key)
    {
        if (dict.TryGetValue(key, out Coroutine existing))
        {
            StopCurrentCoroutine(mono, ref existing);
            dict.Remove(key);
        }
    }
}
