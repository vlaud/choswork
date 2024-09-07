using System.Collections.Generic;
using UnityEngine;
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

    public AudioSource mySoundSpeaker; // �߼Ҹ� �����
    [Header("����Ʈ")]
    public LayerMask KickBlock; // ������ ��� ���̾�
    public GameObject orgDustEff;
    public Player myPlayer;
    public bool noSoundandEffect = false;

    [SerializeField] private List<string> footsteps = new List<string>() { "Player", "Footstep" };
    [SerializeField] private string kick_Whoosh_clip = "Kick_whoosh";
    [SerializeField] private string kick_WallBlock_clip = "Kick_WallBlock";
    [SerializeField] private string AttackedVoice = "Man_Damage_4";

    [SerializeField] private string AttackingVoice = "Man_Damage_1";
    [SerializeField] private string AttackedSound = "face_hit_small_78";
    [SerializeField] private string ScreamVoice = "Man_Damage_Extreme_2";

    public void LeftFootEvent()
    {
        if (!noSoundandEffect && StudyCommandPattern.IsMoveKeyPressed())
        {
            SoundManager.Inst.PlayOneShotRandomClip(mySoundSpeaker, footsteps);
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
            SoundManager.Inst.PlayOneShotRandomClip(mySoundSpeaker, footsteps);
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
            if(col != null) SoundManager.Inst.PlayOneShot(mySoundSpeaker, kick_WallBlock_clip);
            else SoundManager.Inst.PlayOneShot(mySoundSpeaker, kick_Whoosh_clip);
        }
        SoundManager.Inst.PlayOneShot(mySoundSpeaker, kick_Whoosh_clip);
    }

    public void KickCheckStart()
    {
        KickCheck?.Invoke(true);
        if (noSoundandEffect) return;
        SoundManager.Inst.PlayOneShot(mySoundSpeaker, AttackingVoice);
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
        SoundManager.Inst.PlayOneShot(mySoundSpeaker, AttackedSound);
    }

    public void PlayDamage()
    {
        if (noSoundandEffect) return;
        SoundManager.Inst.PlayOneShot(mySoundSpeaker, AttackedVoice);
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
        SoundManager.Inst.PlayOneShot(mySoundSpeaker, ScreamVoice);
    }
}
