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
    public UnityEvent<bool> KickCheck = default;
    // UnityEvent<bool, int> 파라미터 필요한 만큼 넣기
    public Transform leftFoot;
    public Transform rightFoot;
    public AudioClip leftFootSound;// 왼발소리 
    public AudioClip rightFootSound;// 오른발소리 
    public AudioSource myFootSound; // 발소리 오디오
    public GameObject orgDustEff;
    public Player myPlayer;
    public bool noSoundandEffect = false;
    public void LeftFootEvent()
    {
        foreach (KeyCode key in StudyCommandPattern.Inst.Keylist.Keys) // 이동키를 눌러야만 발소리 나오게 설정
        {
            if (myPlayer.myCameras.myCameraState == SpringArms.ViewState.UI) return; // UI상태에선 리턴
            if (Input.GetKey(key))
            {
                SoundManager.Inst.PlayOneShot(myFootSound, leftFootSound);
            }
        }
        
        if (noSoundandEffect) return;
        if (orgDustEff == null) return;
        Instantiate(orgDustEff, leftFoot.position, Quaternion.identity); // 나중에 발바닥 이펙트 추가
        
    }

    public void RightFootEvent()
    {
        foreach (KeyCode key in StudyCommandPattern.Inst.Keylist.Keys)
        {
            if (myPlayer.myCameras.myCameraState == SpringArms.ViewState.UI) return;
            if (Input.GetKey(key))
            {
                SoundManager.Inst.PlayOneShot(myFootSound, rightFootSound);
            }
        }
        
        if (noSoundandEffect) return;
        if (orgDustEff == null) return;
        Instantiate(orgDustEff, rightFoot.position, Quaternion.identity);
    }

    public void OnAttack()
    {
        Attack?.Invoke();
    }
    public void KickCheckStart()
    {
        KickCheck?.Invoke(true);
    }
    public void KickCheckEnd()
    {
        KickCheck?.Invoke(false);
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
