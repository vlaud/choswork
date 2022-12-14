using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Player : BattleSystem
{
    Vector2 targetDir = Vector2.zero;
    public float smoothMoveSpeed = 10.0f;
    public SpringArms myCameras;
    public GameObject KickPoint;
    public enum STATE
    {
        Create, Play, Death
    }
    // Start is called before the first frame update
    void Start()
    {
        myAnim.speed = myStat.MoveSpeed;
        KickPoint.SetActive(false);
    }
   
    // Update is called once per frame
    void Update()
    {
        PlayerMove();
    }
    void PlayerMove()
    {
        float x = 0.0f;
        float z = 0.0f;
        if (myCameras.myCameraState == SpringArms.ViewState.UI) targetDir = Vector2.zero; //UI 상태에선 못움직이게
        else
        {
            targetDir.x = Input.GetAxis("Horizontal");
            targetDir.y = Input.GetAxis("Vertical");
            x = Mathf.Lerp(myAnim.GetFloat("x"), targetDir.x, Time.deltaTime * smoothMoveSpeed);
            z = Mathf.Lerp(myAnim.GetFloat("z"), targetDir.y, Time.deltaTime * smoothMoveSpeed);

            if (Input.GetKey(KeyCode.LeftShift)) myAnim.speed = 1.5f;
            else myAnim.speed = myStat.MoveSpeed;

            //x, z값이 0에 가까우면 0으로 고정
            if (Mathf.Epsilon - 0.01f < x && x < Mathf.Epsilon + 0.01f) x = 0.0f;
            if (Mathf.Epsilon - 0.01f < z && z < Mathf.Epsilon + 0.01f) z = 0.0f;
            myAnim.SetFloat("x", x);
            myAnim.SetFloat("z", z);

            if (Input.GetKeyDown(KeyCode.F) && !myAnim.GetBool("IsKicking")) myAnim.SetTrigger("Kick");
        }
       
    }
    public void KickCheck(bool v)
    {
        KickPoint.SetActive(v);
    }
    public Animator ReturnAnim()
    {
        return myAnim;
    }
}
