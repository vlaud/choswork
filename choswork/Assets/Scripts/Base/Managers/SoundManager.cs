using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SoundManager : Singleton<SoundManager> // ���� �Ŵ����� ��Ŭ�� �������
{
    public AudioSource bgmPlayer = null;

    // Dictionary�� AudioClip ĳ��
    private Dictionary<string, AudioClip> audioClipCache = new Dictionary<string, AudioClip>();
    // ������ ����� ����� Ŭ���� ����� ����Ʈ
    private Dictionary<string, List<AudioClip>> previouslyPlayedClips = new Dictionary<string, List<AudioClip>>();

    // AudioClip�� �����ϴ� �� ���
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
                            // ĳ�ÿ� ����

                            audioClipCache[clip.name] = clip;
                            Debug.Log($"save in {label}: AudioClip: {clip.name}");
                        }
                    }
                }
            };
        }
    }

    // Ư�� ����� Ŭ���� �˻��ϰ� ��ȯ�ϴ� �Լ�
    public void LoadAudioClip(string clipName, System.Action<AudioClip> onClipLoaded)
    {
        // ĳ�ÿ� Ŭ���� ������ �ٷ� ��ȯ
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

        // ĳ�ÿ� ������ �󺧿��� �˻�
        foreach (var label in audioLabels)
        {
            var handle = Addressables.LoadAssetsAsync<AudioClip>(label, null);

            yield return handle; // �񵿱� �۾� �ϷḦ ���

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var clip in handle.Result)
                {
                    if (!audioClipCache.ContainsKey(clip.name))
                    {
                        // ĳ�ÿ� ����
                        audioClipCache[clip.name] = clip;
                    }

                    // ã�� ����� Ŭ���� ��û�� �����̸� ��ȯ
                    if (clip.name == clipName)
                    {
                        Debug.Log($"{label}: Found AudioClip: {clip.name} in label {label}");
                        onClipLoaded?.Invoke(clip);
                        clipFound = true;
                        break; // Ŭ���� ã�����Ƿ� ������ Ŭ�� �˻��� �ߴ�
                    }
                }
            }

            // Ŭ���� ã������ ������ �� �˻� �ߴ�
            if (clipFound)
                break;
        }

        // Ŭ���� ã�� ������ ���
        if (!clipFound)
        {
            Debug.LogError($"AudioClip '{clipName}' not found in any label.");
        }
    }

    // ����� Ŭ���� ����ϴ� �Լ� (����)
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

    public void PlayOneShot(AudioSource audioSource, string clipName) // ȿ���� �ѹ��� ������
    {
        LoadAudioClip(clipName, clip =>
        {
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        });
    }

    // ���� AND �������� �����ϴ� Ŭ�� �� �ϳ��� ���
    public void PlayRandomClip(AudioSource audioSource, List<string> labels)
    {
        // Addressables���� ���� ���� �̿��� ���� �ε�
        Addressables.LoadAssetsAsync<AudioClip>(labels, null).Completed += OnClipsLoaded;

        // Ŭ�� �ε� �Ϸ� �� ó��
        void OnClipsLoaded(AsyncOperationHandle<IList<AudioClip>> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                IList<AudioClip> clips = handle.Result;

                if (clips.Count > 0)
                {
                    // ������ ����� Ŭ���� ������ �ΰ�, �ߺ� ���������� ���
                    AudioClip previousClip = audioSource.clip;
                    AudioClip randomClip;

                    // �ߺ����� �ʵ��� ���� Ŭ�� ����
                    do
                    {
                        randomClip = clips[Random.Range(0, clips.Count)];
                    }
                    while (randomClip == previousClip && clips.Count > 1); // Ŭ���� �ϳ��� ������ ��� ���� ����

                    // ����� �ҽ��� Ŭ�� ���� �� ���
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

    // ���� AND �������� �����ϴ� Ŭ�� �� �ϳ��� ���
    public void PlayOneShotRandomClip(AudioSource audioSource, List<string> labels)
    {
        // Addressables���� ���� ���� �̿��� ���� �ε�
        Addressables.LoadAssetsAsync<AudioClip>(labels, null, Addressables.MergeMode.Intersection).Completed += OnClipsLoaded;

        // Ŭ�� �ε� �Ϸ� �� ó��
        void OnClipsLoaded(AsyncOperationHandle<IList<AudioClip>> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                IList<AudioClip> clips = handle.Result;

                if (clips.Count > 0)
                {
                    // ������ ����� Ŭ���� ������ �ΰ�, �ߺ� ���������� ���
                    AudioClip previousClip = audioSource.clip;
                    AudioClip randomClip;

                    // �ߺ����� �ʵ��� ���� Ŭ�� ����
                    do
                    {
                        randomClip = clips[Random.Range(0, clips.Count)];
                    }
                    while (randomClip == previousClip && clips.Count > 1); // Ŭ���� �ϳ��� ������ ��� ���� ����

                    // ����� �ҽ��� Ŭ�� ���� �� ���
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
