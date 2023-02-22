using UnityEngine;
using UnityEngine.Events;

public class CarAnimEvent : MonoBehaviour
{
    public UnityEvent CarOpen;
    public UnityEvent CarClose;

    public void OnCarOpen()
    {
        CarOpen?.Invoke();
    }
    public void OnCarClose()
    {
        CarClose?.Invoke();
    }
}
