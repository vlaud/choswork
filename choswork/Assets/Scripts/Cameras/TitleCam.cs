using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCam : MonoBehaviour
{
    public CameraSet titleCam;
    public float LookSpeed = 2.0f;
    public float _limit = 0.15f;
    public Vector2 camPos = Vector2.zero;
    public Vector2 desireScreenPos = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        titleCam.myCam.transform.localPosition = camPos;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        //if (Input.GetMouseButton(1))
        {
            desireScreenPos.x = mousePos.x;
            desireScreenPos.x = Mathf.Clamp(desireScreenPos.x, -_limit, _limit);
            desireScreenPos.y = mousePos.y;
            desireScreenPos.y = Mathf.Clamp(desireScreenPos.y, -_limit * 0.5f, _limit * 0.5f);

            camPos = Vector2.Lerp(camPos, desireScreenPos, Time.deltaTime * LookSpeed);
        }
        titleCam.myCam.transform.localPosition = camPos;
    }
}
