using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� �ڷ�ƾ�� ������ Ŭ����
/// </summary>
public class CoroutineRunner
{
    private MonoBehaviour currentMono;

    public CoroutineRunner(MonoBehaviour currentMono)
    {
        this.currentMono = currentMono;
    }

    public void StopCurrentCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            currentMono.StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public void StartCurrentCoroutine(Coroutine currentCoroutine, out Coroutine coroutine, IEnumerator coAction)
    {
        if (currentCoroutine == null)
        {
            coroutine = currentMono.StartCoroutine(coAction);
            return;
        }
        
        coroutine = currentCoroutine;
        StopCurrentCoroutine(coroutine);
        coroutine = currentMono.StartCoroutine(coAction);
    }

}
