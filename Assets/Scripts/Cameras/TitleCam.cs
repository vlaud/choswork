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
        titleCam.realCam.transform.localPosition = camPos;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        float width = Screen.width * 0.5f;
        float height = Screen.height * 0.5f;
        //if (Input.GetMouseButton(1))
        {
            desireScreenPos.x = _limit * ((mousePos.x - width) / width);
            desireScreenPos.x = Mathf.Clamp(desireScreenPos.x, -_limit, _limit);
            desireScreenPos.y = _limit * ((mousePos.y - height) / height);
            desireScreenPos.y = Mathf.Clamp(desireScreenPos.y, -_limit, _limit);
            camPos = Vector2.Lerp(camPos, desireScreenPos, Time.deltaTime * LookSpeed);
        }
        titleCam.realCam.transform.localPosition = camPos;
    }
}
