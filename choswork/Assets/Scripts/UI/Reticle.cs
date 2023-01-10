using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
public class Reticle : MonoBehaviour
{
    private RectTransform reticle;

    public Player myPlayer;
    public float restingSize;
    public float maxSize;
    public float speed;
    private float currentSize;
    private float animOffset = 0.3f;
    //[Range(50f, 250f)]
    //public float size;
    private void Start()
    {
        reticle = GetComponent<RectTransform>();
    }
    private void Update()
    {
        float x, z;
        x = myPlayer.ReturnAnim().GetFloat("x");
        z = myPlayer.ReturnAnim().GetFloat("z");

        //reticle.sizeDelta = new Vector2(size, size);
        if (Mathf.Epsilon - animOffset < x && x < Mathf.Epsilon + animOffset &&
           Mathf.Epsilon - animOffset < z && z < Mathf.Epsilon + animOffset)
        {
            currentSize = Mathf.Lerp(currentSize, restingSize, speed * Time.deltaTime); // 안움직일때
        }
        else
        {
            currentSize = Mathf.Lerp(currentSize, maxSize, speed * Time.deltaTime); // 움직일때
        }
        reticle.sizeDelta = new Vector2(currentSize, currentSize);
    }
}
