using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    
    public AudioSource bgmPlayer = null;

    public float bgmVolume
    {
        get => bgmPlayer.volume;
        set
        {
            bgmPlayer.volume = Mathf.Clamp(value, 0.0f, 1.0f);
            //PlayerPrefs.SetInt();
            //PlayerPrefs.SetString();
            PlayerPrefs.SetFloat("Game_BGM_Volume", 1.0f - bgmPlayer.volume);
        }

    }

    public float bgmPitch
    {
        get => bgmPlayer.pitch;
        set => bgmPlayer.pitch = value;
    }

    float _effectvolume = 1.0f;
    public float effectVolume
    {
        get => _effectvolume;
        set
        {
            _effectvolume = Mathf.Clamp(value, 0.0f, 1.0f);
            PlayerPrefs.SetFloat("Game_Effect_Volume", 1.0f - _effectvolume);
        }
    }


    private void Awake()
    {
        base.Initialize();
        if(bgmPlayer != null)
            bgmPlayer.volume = 1.0f - PlayerPrefs.GetFloat("Game_BGM_Volume");
        _effectvolume = 1.0f - PlayerPrefs.GetFloat("Game_Effect_Volume");

    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        bgmPlayer.clip = clip;
        bgmPlayer.loop = loop;
        bgmPlayer.Play();
    }

    public void PauseBGM()
    {
        bgmPlayer.Pause();
    }

    public void ResumeBGM()
    {
        bgmPlayer.Play();
    }
    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    public void PlayOneShot(AudioSource audio, AudioClip clip)
    {
        audio.volume = effectVolume;
        audio.PlayOneShot(clip);
    }
    
}
