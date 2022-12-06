using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
    public UnityEvent Attack = default;
    public UnityEvent Skill = default;
    public UnityEvent<bool> ComboCheck = default;
    // UnityEvent<bool, int> 파라미터 필요한 만큼 넣기
    public Transform leftFoot;
    public Transform rightFoot;
    public GameObject orgDustEff;
    public void LeftFootEvent()
    {
        Instantiate(orgDustEff, leftFoot.position, Quaternion.identity);
        
    }

    public void RightFootEvent()
    {
        Instantiate(orgDustEff, rightFoot.position, Quaternion.identity);
       
    }

    public void OnAttack()
    {
       
        Attack?.Invoke();
    }

    public void Onskill()
    {
        Skill?.Invoke();
    }

    public void ComboCheckStart()
    {
        ComboCheck?.Invoke(true);
    }
    public void ComboCheckEnd()
    {
        ComboCheck?.Invoke(false);
    }
}
