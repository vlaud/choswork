using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FaderAnimation : MonoBehaviour
{
    public UnityEvent LoadScene = default;

    public void OnLoadScene()
    {
        LoadScene?.Invoke();
    }
}
