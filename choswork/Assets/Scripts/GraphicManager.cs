using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class GraphicManager : Singleton<GraphicManager> // 사운드 매니저는 싱클톤 방식으로
{
    public PostProcessProfile brightness;
    public PostProcessProfile constrast;
    public PostProcessLayer layer;

    AutoExposure exposure;
    ColorGrading grading;

    public float fbrightness
    {
        get => exposure.keyValue.value;
        set
        {
            exposure.keyValue.value = Mathf.Clamp(value, 0.1f, 10.0f);
            PlayerPrefs.SetFloat("Game_Graphic_Brightness", 1.0f - exposure.keyValue.value);
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
        if (brightness.TryGetSettings(out exposure))
        {
            exposure.keyValue.value = 1.0f - PlayerPrefs.GetFloat("Game_Graphic_Brightness");
        }
        if (constrast.TryGetSettings(out grading))
        {
            grading.contrast.value = 1.0f - PlayerPrefs.GetFloat("Game_Graphic_Contrast");
        }
    }
    public void AdjustBrightness(float value)
    {
        if(value != 0)
        {
            exposure.keyValue.value = value;
        }
        else
        {
            exposure.keyValue.value = .05f;
        }
    }
    public void AdjustContrast(float value)
    {
        if (value != 0)
        {
            grading.contrast.value = value;
        }
        else
        {
            grading.contrast.value = .05f;
        }
    }
}
