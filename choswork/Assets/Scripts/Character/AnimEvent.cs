using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
    public UnityEvent Attack = default;
    public UnityEvent Kick = default;
    public UnityEvent Skill = default;
    public UnityEvent<bool> ComboCheck = default;
    public UnityEvent<bool> KickCheck = default;
    // UnityEvent<bool, int> 파라미터 필요한 만큼 넣기
    public Transform leftFoot;
    public Transform rightFoot;
    public AudioClip leftFootSound;// 왼발소리 
    public AudioClip rightFootSound;// 오른발소리 
    public AudioClip kickSound;// 발차기 소리
    public AudioClip kickWallSound;// 닿는 발차기 소리
    public AudioSource myFootSound; // 발소리 오디오
    public LayerMask KickBlock; // 발차기 닿는 레이어
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
    public void OnKick()
    {
        Kick?.Invoke();
    }
    public void PlayKickSound()
    {
        float rad = myPlayer.GetComponent<CapsuleCollider>().radius 
            + myPlayer.KickPoint.GetComponent<SphereCollider>().radius
            + 0.5f;
        Collider[] list = Physics.OverlapSphere(transform.position, rad, KickBlock);
        // 숫자가 다른 이유 : 애니메이션 진행도에 따라 발 위치가 다름 = 부딪히는 소리가 먼저 실행되어서 범위를 늘려줘야함
        foreach (Collider col in list)
        {
            if(col != null) SoundManager.Inst.PlayOneShot(myFootSound, kickWallSound);
            else SoundManager.Inst.PlayOneShot(myFootSound, kickSound);
        }
        SoundManager.Inst.PlayOneShot(myFootSound, kickSound);
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
