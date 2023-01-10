using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    public RectTransform myHPBar;
    public Vector2 size;

    public float Width_Base;
    public float Width;
    public float GetValue;
    void Start()
    {
        size = myHPBar.sizeDelta;
        Width = Width_Base = size.x;
        GetValue = Width / Width_Base;
    }

    void Update()
    {
        Width = GetValue * Width_Base;
        size.x = Width;
        myHPBar.sizeDelta = size;
    }
}
