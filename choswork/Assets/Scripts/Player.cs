using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Player : BattleSystem
{
    Vector2 targetDir = Vector2.zero;
    Vector3 deltaPosition = Vector3.zero;
    Quaternion deltaRotation = Quaternion.identity;
    public float smoothMoveSpeed = 10.0f;
    public Transform myRoot;
    
    public enum STATE
    {
        Create, Play, Death
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }
   
    // Update is called once per frame
    void Update()
    {
        float delta = myStat.MoveSpeed * Time.deltaTime;
        Vector3 dir = Vector3.zero;
        
        targetDir.x = Input.GetAxis("Horizontal");
        targetDir.y = Input.GetAxis("Vertical");
        
        float x = Mathf.Lerp(myAnim.GetFloat("x"), targetDir.x, Time.deltaTime * smoothMoveSpeed);
        float y = Mathf.Lerp(myAnim.GetFloat("z"), targetDir.y, Time.deltaTime * smoothMoveSpeed);
        myAnim.SetFloat("x", x);
        myAnim.SetFloat("z", y);
        dir.x = x;
        dir.z = y;
        transform.Translate(dir * delta, Space.World);
        //transform.rotation = dir * myRoot.rotation;
    }
}
