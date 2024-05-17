using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class GraphicManager : Singleton<GraphicManager> // 사운드 매니저는 싱클톤 방식으로
{
    public PostProcessProfile brightness;
    public PostProcessProfile constrast;
    public PostProcessLayer layer;

    ColorGrading grading;

    public float fbrightness
    {
        get => grading.postExposure.value;
        set
        {
            grading.postExposure.value = Mathf.Clamp(value, -3f, 3f);
            PlayerPrefs.SetFloat("Game_Graphic_Brightness", 1.0f - grading.postExposure.value);
        }
    }
    public float fconstrast
    {
        get => grading.contrast.value;
        set
        {
            grading.contrast.value = Mathf.Clamp(value, -60f, 60f);
            PlayerPrefs.SetFloat("Game_Graphic_Contrast", 1.0f - grading.contrast.value);
        }
    }
    private void Awake()
    {
        base.Initialize();
        if (constrast.TryGetSettings(out grading))
        {
            grading.postExposure.value = 1.0f - PlayerPrefs.GetFloat("Game_Graphic_Brightness");
            grading.contrast.value = 1.0f - PlayerPrefs.GetFloat("Game_Graphic_Contrast");
        }
    }
}
