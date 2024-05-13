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
    // UnityEvent<bool, int> �Ķ���� �ʿ��� ��ŭ �ֱ�
    public Transform leftFoot;
    public Transform rightFoot;
    [Header("����")]
    public AudioClip leftFootSound;// �޹߼Ҹ� 
    public AudioClip rightFootSound;// �����߼Ҹ� 
    public AudioClip kickSound;// ������ �Ҹ�
    public AudioClip kickWallSound;// ��� ������ �Ҹ�
    public AudioClip MobAttackSound;// �� ���� ȿ����
    public AudioClip MobAttackYell;// �� ���� ����
    public AudioClip DamageSound; // ������ ����
    public AudioClip ScreamSound;// ��� ������ �Ҹ�
    public AudioSource mySoundSpeaker; // �߼Ҹ� �����
    [Header("����Ʈ")]
    public LayerMask KickBlock; // ������ ��� ���̾�
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
            Instantiate(orgDustEff, leftFoot.position, Quaternion.identity); // ���߿� �߹ٴ� ����Ʈ �߰�
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
        // ������ �Ҹ� ��ġ(KickTransform)���� ����
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
