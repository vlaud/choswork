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
    public UnityEvent<bool> BleedCheck = default;
    // UnityEvent<bool, int> 파라미터 필요한 만큼 넣기
    public Transform leftFoot;
    public Transform rightFoot;
    [Header("사운드")]
    public AudioClip leftFootSound;// 왼발소리 
    public AudioClip rightFootSound;// 오른발소리 
    public AudioClip kickSound;// 발차기 소리
    public AudioClip kickWallSound;// 닿는 발차기 소리
    public AudioClip MobAttackSound;// 몹 공격 효과음
    public AudioClip MobAttackYell;// 몹 공격 기합
    public AudioClip DamageSound; // 데미지 신음
    public AudioClip ScreamSound;// 닿는 발차기 소리
    public AudioSource mySoundSpeaker; // 발소리 오디오
    [Header("이펙트")]
    public LayerMask KickBlock; // 발차기 닿는 레이어
    public GameObject orgDustEff;
    public Player myPlayer;
    public bool noSoundandEffect = false;

    public void LeftFootEvent()
    {
        if (!noSoundandEffect && StudyCommandPattern.IsMoveKeyPressed())
        {
            SoundManager.Inst.PlayOneShot(mySoundSpeaker, leftFootSound);
        }
        if (orgDustEff != null)
        {
            Instantiate(orgDustEff, leftFoot.position, Quaternion.identity); // 나중에 발바닥 이펙트 추가
        }
    }

    public void RightFootEvent()
    {
        if (!noSoundandEffect && StudyCommandPattern.IsMoveKeyPressed())
        {
            SoundManager.Inst.PlayOneShot(mySoundSpeaker, rightFootSound);
        }
        if (orgDustEff != null)
        {
            Instantiate(orgDustEff, rightFoot.position, Quaternion.identity);
        }
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
        if (noSoundandEffect) return;
        SoundManager.Inst.PlayOneShot(mySoundSpeaker, MobAttackYell);
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
    public void PlayHit()
    {
        if (noSoundandEffect) return;
        SoundManager.Inst.PlayOneShot(mySoundSpeaker, MobAttackSound);
    }
    public void PlayDamage()
    {
        if (noSoundandEffect) return;
        SoundManager.Inst.PlayOneShot(mySoundSpeaker, DamageSound);
    }
    public void StartBleed()
    {
        BleedCheck?.Invoke(true);
    }
    public void EndBleed()
    {
        BleedCheck?.Invoke(false);
    }
    public void PlayScream()
    {
        if (noSoundandEffect) return;
        SoundManager.Inst.PlayOneShot(mySoundSpeaker, ScreamSound);
    }
}
