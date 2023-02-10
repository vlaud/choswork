using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneLoader : Singleton<SceneLoader>
{
    //public static SceneLoader Inst = null;
    bool isChange = false;
    private void Awake()
    {
        //Inst = this;
        base.Initialize();
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(int i)
    {
        if(!isChange)
            StartCoroutine(Loading(i));
    }
    public void ChangeScene(string scene)
    {
        if (!isChange)
            StartCoroutine(Loading(scene));
    }
    IEnumerator Loading(int i)
    {
        isChange = true;
        yield return SceneManager.LoadSceneAsync(2);
        GameObject obj = GameObject.Find("LoadingGage");
        Slider slider = obj.GetComponent<Slider>();
        slider.value = 0.0f;
        yield return StartCoroutine(LoadingTarget(slider, i));
        isChange = false;
    }
    IEnumerator Loading(string scene)
    {
        isChange = true;
        yield return SceneManager.LoadSceneAsync(2);
        GameObject obj = GameObject.Find("LoadingGage");
        Slider slider = obj.GetComponent<Slider>();
        slider.value = 0.0f;
        yield return StartCoroutine(LoadingTarget(slider, scene));
        isChange = false;
    }
    IEnumerator LoadingTarget(Slider slider, int i)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(i);
        // 씬로딩이 끝나기 전까진 씬을 활성화 시키지 않음
        ao.allowSceneActivation = false;

        while(!ao.isDone)
        {
            slider.value = ao.progress / 0.9f;
            if(Mathf.Approximately(slider.value, 1.0f))
            {
                // 씬로딩이 끝났으므로 씬 활성화
                ao.allowSceneActivation = true;
            }
            yield return null;
        }
    }
    IEnumerator LoadingTarget(Slider slider, string scene)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
        // 씬로딩이 끝나기 전까진 씬을 활성화 시키지 않음
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            slider.value = ao.progress / 0.9f;
            if (Mathf.Approximately(slider.value, 1.0f))
            {
                // 씬로딩이 끝났으므로 씬 활성화
                ao.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
