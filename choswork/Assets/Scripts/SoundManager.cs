using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> // 사운드 매니저는 싱클톤 방식으로
{
    public AudioSource bgmPlayer = null;
    public List<AudioSource> soundPlayer = null;

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
    public float soundVolume
    {
        get => soundPlayer[0].volume;
        set
        {
            soundPlayer[0].volume = Mathf.Clamp(value, 0.0f, 1.0f);
            PlayerPrefs.SetFloat("Game_Sound_Volume", 1.0f - bgmPlayer.volume);
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

        GameObject[] objs = GameObject.FindGameObjectsWithTag("SoundSpeaker");
        for (int i = 0; i < objs.Length; ++i)
        {
            soundPlayer.Add(objs[i].GetComponent<AudioSource>());
        }
        
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

    public void PlayOneShot(AudioSource audio, AudioClip clip) // 효과음 한번만 나오게
    {
        audio.volume = effectVolume;
        audio.PlayOneShot(clip);
    }
    
}
