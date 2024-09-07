using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SoundManager : Singleton<SoundManager> // 사운드 매니저는 싱클톤 방식으로
{
    public AudioSource bgmPlayer = null;

    // Dictionary로 AudioClip 캐싱
    private Dictionary<string, AudioClip> audioClipCache = new Dictionary<string, AudioClip>();
    // 이전에 재생한 오디오 클립을 기록할 리스트
    private Dictionary<string, List<AudioClip>> previouslyPlayedClips = new Dictionary<string, List<AudioClip>>();

    // AudioClip이 존재하는 라벨 목록
    private List<string> audioLabels = new List<string> { "BGM", "Mobs", "Player", "Damage", "Footstep" };

    public float bgmVolume
    {
        get => bgmPlayer.volume;
        set
        {
            bgmPlayer.volume = Mathf.Clamp(value, 0.0f, 1.0f);
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
        if (bgmPlayer != null)
            bgmPlayer.volume = 1.0f - PlayerPrefs.GetFloat("Game_BGM_Volume");
        _effectvolume = 1.0f - PlayerPrefs.GetFloat("Game_Effect_Volume");
        LoadAll_AudioClips();
    }

    public void PlayBGM(string clipName, bool loop = true)
    {
        LoadAudioClip(clipName, clip =>
        {
            if (clip != null)
            {
                bgmPlayer.clip = clip;
                bgmPlayer.loop = loop;
                bgmPlayer.Play();
            }
        });
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

    public void LoadAll_AudioClips()
    {
        foreach (var label in audioLabels)
        {
            Addressables.LoadAssetsAsync<AudioClip>(label, null).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    foreach (var clip in handle.Result)
                    {
                        if (!audioClipCache.ContainsKey(clip.name))
                        {
                            // 캐시에 저장

                            audioClipCache[clip.name] = clip;
                            Debug.Log($"save in {label}: AudioClip: {clip.name}");
                        }
                    }
                }
            };
        }
    }

    // 특정 오디오 클립을 검색하고 반환하는 함수
    public void LoadAudioClip(string clipName, System.Action<AudioClip> onClipLoaded)
    {
        // 캐시에 클립이 있으면 바로 반환
        if (audioClipCache.TryGetValue(clipName, out AudioClip cachedClip))
        {
            Debug.Log($"AudioClip '{clipName}' found in cache.");
            onClipLoaded?.Invoke(cachedClip);
            return;
        }

        StartCoroutine(LoadAudioClipFromLabelsCoroutine(clipName, onClipLoaded));
    }

    private IEnumerator LoadAudioClipFromLabelsCoroutine(string clipName, System.Action<AudioClip> onClipLoaded)
    {
        bool clipFound = false;

        // 캐시에 없으면 라벨에서 검색
        foreach (var label in audioLabels)
        {
            var handle = Addressables.LoadAssetsAsync<AudioClip>(label, null);

            yield return handle; // 비동기 작업 완료를 대기

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var clip in handle.Result)
                {
                    if (!audioClipCache.ContainsKey(clip.name))
                    {
                        // 캐시에 저장
                        audioClipCache[clip.name] = clip;
                    }

                    // 찾은 오디오 클립이 요청한 파일이면 반환
                    if (clip.name == clipName)
                    {
                        Debug.Log($"{label}: Found AudioClip: {clip.name} in label {label}");
                        onClipLoaded?.Invoke(clip);
                        clipFound = true;
                        break; // 클립을 찾았으므로 나머지 클립 검색을 중단
                    }
                }
            }

            // 클립을 찾았으면 나머지 라벨 검색 중단
            if (clipFound)
                break;
        }

        // 클립을 찾지 못했을 경우
        if (!clipFound)
        {
            Debug.LogError($"AudioClip '{clipName}' not found in any label.");
        }
    }

    // 오디오 클립을 재생하는 함수 (예시)
    public void PlayClip(AudioSource audioSource, string clipName)
    {
        LoadAudioClip(clipName, clip =>
        {
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        });
    }

    public void PlayOneShot(AudioSource audioSource, string clipName) // 효과음 한번만 나오게
    {
        LoadAudioClip(clipName, clip =>
        {
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        });
    }

    // 라벨을 AND 조건으로 만족하는 클립 중 하나를 재생
    public void PlayRandomClip(AudioSource audioSource, List<string> labels)
    {
        // Addressables에서 여러 라벨을 이용해 에셋 로드
        Addressables.LoadAssetsAsync<AudioClip>(labels, null).Completed += OnClipsLoaded;

        // 클립 로드 완료 후 처리
        void OnClipsLoaded(AsyncOperationHandle<IList<AudioClip>> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                IList<AudioClip> clips = handle.Result;

                if (clips.Count > 0)
                {
                    // 이전에 재생한 클립을 저장해 두고, 중복 방지용으로 사용
                    AudioClip previousClip = audioSource.clip;
                    AudioClip randomClip;

                    // 중복되지 않도록 랜덤 클립 선택
                    do
                    {
                        randomClip = clips[Random.Range(0, clips.Count)];
                    }
                    while (randomClip == previousClip && clips.Count > 1); // 클립이 하나만 있으면 계속 선택 방지

                    // 오디오 소스에 클립 설정 및 재생
                    audioSource.clip = randomClip;
                    audioSource.Play();
                }
                else
                {
                    Debug.LogWarning("No audio clips found for the specified labels.");
                }
            }
            else
            {
                Debug.LogError("Failed to load audio clips.");
            }
        }
    }

    // 라벨을 AND 조건으로 만족하는 클립 중 하나를 재생
    public void PlayOneShotRandomClip(AudioSource audioSource, List<string> labels)
    {
        // Addressables에서 여러 라벨을 이용해 에셋 로드
        Addressables.LoadAssetsAsync<AudioClip>(labels, null, Addressables.MergeMode.Intersection).Completed += OnClipsLoaded;

        // 클립 로드 완료 후 처리
        void OnClipsLoaded(AsyncOperationHandle<IList<AudioClip>> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                IList<AudioClip> clips = handle.Result;

                if (clips.Count > 0)
                {
                    // 이전에 재생한 클립을 저장해 두고, 중복 방지용으로 사용
                    AudioClip previousClip = audioSource.clip;
                    AudioClip randomClip;

                    // 중복되지 않도록 랜덤 클립 선택
                    do
                    {
                        randomClip = clips[Random.Range(0, clips.Count)];
                    }
                    while (randomClip == previousClip && clips.Count > 1); // 클립이 하나만 있으면 계속 선택 방지

                    // 오디오 소스에 클립 설정 및 재생
                    audioSource.clip = randomClip;
                    audioSource.PlayOneShot(randomClip);
                }
                else
                {
                    Debug.LogWarning("No audio clips found for the specified labels.");
                }
            }
            else
            {
                Debug.LogError("Failed to load audio clips.");
            }
        }
    }
}
