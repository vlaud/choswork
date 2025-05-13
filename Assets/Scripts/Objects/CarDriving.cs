using UnityEngine;
using UnityEngine.Events;

public class CarDriving : MonoBehaviour
{
    public UnityEvent CarOut;
   
    public void OnCarOut()
    {
        CarOut?.Invoke();
    }
}
