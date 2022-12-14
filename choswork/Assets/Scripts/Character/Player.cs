using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Player : BattleSystem
{
    Vector2 targetDir = Vector2.zero;
    public float smoothMoveSpeed = 10.0f;
    public float myMoveSpeed = 1.0f;
    public SpringArms myCameras;
    public GameObject KickPoint;
    public bool IsWall = false;
    public enum STATE
    {
        Create, Play, Death
    }
    // Start is called before the first frame update
    void Start()
    {
        myMoveSpeed = myStat.MoveSpeed;
        KickPoint.SetActive(false);
    }
   
    // Update is called once per frame
    void Update()
    {
        PlayerMove();
    }
    void PlayerMove()
    {
        myAnim.speed = myMoveSpeed;

        float x = 0.0f;
        float z = 0.0f;
        if (myCameras.myCameraState == SpringArms.ViewState.UI) targetDir = Vector2.zero; //UI 상태에선 못움직이게
        else
        {
            targetDir.x = Input.GetAxisRaw("Horizontal");
            targetDir.y = Input.GetAxisRaw("Vertical");
            x = Mathf.Lerp(myAnim.GetFloat("x"), targetDir.x, Time.deltaTime * smoothMoveSpeed);
            z = Mathf.Lerp(myAnim.GetFloat("z"), targetDir.y, Time.deltaTime * smoothMoveSpeed);

            if (Input.GetKey(KeyCode.LeftShift) && !IsWall) myMoveSpeed = 1.5f; // 벽 충돌시엔 질주 off
            else myMoveSpeed = myStat.MoveSpeed;
           
            //x, z값이 0에 가까우면 0으로 고정
            if (Mathf.Epsilon - 0.01f < x && x < Mathf.Epsilon + 0.01f) x = 0.0f;
            if (Mathf.Epsilon - 0.01f < z && z < Mathf.Epsilon + 0.01f) z = 0.0f;
            myAnim.SetFloat("x", x);
            myAnim.SetFloat("z", z);

            if (Input.GetKeyDown(KeyCode.F) && !myAnim.GetBool("IsKicking")) myAnim.SetTrigger("Kick");
        }

    }
    
    private void OnCollisionStay(Collision collision) // 벽 충돌시엔 질주 off
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            //myMoveSpeed = 0.5f; 벽충돌 이동속도는 나중에 수정
            IsWall = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            //myMoveSpeed = myStat.MoveSpeed;
            IsWall = false;
        }
    }
    public void KickTarget()
    {
        Collider[] list = Physics.OverlapSphere(KickPoint.transform.position, 0.2f, 1 << LayerMask.NameToLayer("Enemy"));
        foreach (Collider col in list)
        {
            Debug.Log(col);
            col.GetComponent<Monster>().GetKick();
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
