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
    public AudioSource myFootSound; // �߼Ҹ� �����
    public LayerMask KickBlock; // ������ ��� ���̾�
    public GameObject orgDustEff;
    public Player myPlayer;
    public bool noSoundandEffect = false;
    public void LeftFootEvent()
    {
        foreach (KeyCode key in StudyCommandPattern.Inst.Keylist.Keys) // �̵�Ű�� �����߸� �߼Ҹ� ������ ����
        {
            if (myPlayer.myCameras.myCameraState == SpringArms.ViewState.UI) return; // UI���¿��� ����
            if (Input.GetKey(key))
            {
                SoundManager.Inst.PlayOneShot(myFootSound, leftFootSound);
            }
        }
        
        if (noSoundandEffect) return;
        if (orgDustEff == null) return;
        Instantiate(orgDustEff, leftFoot.position, Quaternion.identity); // ���߿� �߹ٴ� ����Ʈ �߰�
        
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
        Collider[] list = Physics.OverlapSphere(myPlayer.KickPoint.transform.position, 1.1f, KickBlock);
        // �׳� LayerMask.NameToLayer("Enemy"))�� �ϸ� ���̾ �����Ѱ� ���õȴ� 
        foreach (Collider col in list)
        {
            Debug.Log(col);
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
