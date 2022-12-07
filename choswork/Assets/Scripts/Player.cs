using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Player : BattleSystem
{
    Vector2 targetDir = Vector2.zero;
    public float smoothMoveSpeed = 10.0f;
    public SpringArms myCameras;
    public enum STATE
    {
        Create, Play, Death
    }
    // Start is called before the first frame update
    void Start()
    {
        myAnim.speed = myStat.MoveSpeed;
    }
   
    // Update is called once per frame
    void Update()
    {
        targetDir.x = Input.GetAxis("Horizontal");
        targetDir.y = Input.GetAxis("Vertical");
        
        float x = Mathf.Lerp(myAnim.GetFloat("x"), targetDir.x, Time.deltaTime * smoothMoveSpeed);
        float y = Mathf.Lerp(myAnim.GetFloat("z"), targetDir.y, Time.deltaTime * smoothMoveSpeed);
        if(Input.GetKey(KeyCode.LeftShift))
        {
            myAnim.speed = 1.5f;
        }
        else
        {
            myAnim.speed = myStat.MoveSpeed;
        }
        myAnim.SetFloat("x", x);
        myAnim.SetFloat("z", y);
    }
    
}
