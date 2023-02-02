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
    // UnityEvent<bool, int> �Ķ���� �ʿ��� ��ŭ �ֱ�
    public Transform leftFoot;
    public Transform rightFoot;
    public AudioClip leftFootSound;// �޹߼Ҹ� 
    public AudioClip rightFootSound;// �����߼Ҹ� 
    public AudioClip kickSound;// ������ �Ҹ�
    public AudioClip kickWallSound;// ��� ������ �Ҹ�
    public AudioClip monsterScreamSound;// ��� ������ �Ҹ�
    public AudioSource mySoundSpeaker; // �߼Ҹ� �����
    public LayerMask KickBlock; // ������ ��� ���̾�
    public GameObject orgDustEff;
    public Player myPlayer;
    public bool noSoundandEffect = false;
    public void LeftFootEvent()
    {
        if (noSoundandEffect) return;
        foreach (KeyCode key in StudyCommandPattern.Inst.Keylist.Keys) // �̵�Ű�� �����߸� �߼Ҹ� ������ ����
        {
            if (myPlayer.myCameras.myCameraState == SpringArms.ViewState.UI) return; // UI���¿��� ����
            if (Input.GetKey(key))
            {
                SoundManager.Inst.PlayOneShot(mySoundSpeaker, leftFootSound);
            }
        }
        if (orgDustEff == null) return;
        Instantiate(orgDustEff, leftFoot.position, Quaternion.identity); // ���߿� �߹ٴ� ����Ʈ �߰�
        
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
