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
    public AudioClip monsterScreamSound;// 닿는 발차기 소리
    public AudioSource mySoundSpeaker; // 발소리 오디오
    public LayerMask KickBlock; // 발차기 닿는 레이어
    public GameObject orgDustEff;
    public Player myPlayer;
    public bool noSoundandEffect = false;
    public void LeftFootEvent()
    {
        if (noSoundandEffect) return;
        foreach (KeyCode key in StudyCommandPattern.Inst.Keylist.Keys) // 이동키를 눌러야만 발소리 나오게 설정
        {
            if (myPlayer.myCameras.myCameraState == SpringArms.ViewState.UI) return; // UI상태에선 리턴
            if (Input.GetKey(key))
            {
                SoundManager.Inst.PlayOneShot(mySoundSpeaker, leftFootSound);
            }
        }
        if (orgDustEff == null) return;
        Instantiate(orgDustEff, leftFoot.position, Quaternion.identity); // 나중에 발바닥 이펙트 추가
        
    }

    public void RightFootEvent()
    {
        if (noSoundandEffect) return;
        foreach (KeyCode key in StudyCommandPattern.Inst.Keylist.Keys)
        {
            if (myPlayer.myCameras.myCameraState == SpringArms.ViewState.UI) return;
            if (Input.GetKey(key))
            {
                SoundManager.Inst.PlayOneShot(mySoundSpeaker, rightFootSound);
            }
        }
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
        if (noSoundandEffect) return;
        float rad = myPlayer.KickPoint.GetComponent<SphereCollider>().radius;
            
        Collider[] list = Physics.OverlapSphere(myPlayer.KickTransform.position, rad, KickBlock);
        // 발차기 소리 위치(KickTransform)에서 실행
        foreach (Collider col in list)
        {
            if(col != null) SoundManager.Inst.PlayOneShot(mySoundSpeaker, kickWallSound);
            else SoundManager.Inst.PlayOneShot(mySoundSpeaker, kickSound);
        }
        SoundManager.Inst.PlayOneShot(mySoundSpeaker, kickSound);
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
    public void PlayScream()
    {
        if (noSoundandEffect) return;
        SoundManager.Inst.PlayOneShot(mySoundSpeaker, monsterScreamSound);
    }
}
