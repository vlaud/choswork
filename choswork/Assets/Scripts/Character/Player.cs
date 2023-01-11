using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Player : BattleSystem
{
    Vector2 targetDir = Vector2.zero;
    public float smoothMoveSpeed = 10.0f;
    public float myMoveSpeed = 1.0f;
    public float KickStrength = 10000.0f;
    public SpringArms myCameras;
    public GameObject KickPoint; // ���� ������ ȿ�� ��ġ
    public Transform KickTransform; // ������ �Ҹ� ��ġ
    public bool IsWall = false;

    private float animOffset = 0.01f;
    public enum STATE
    {
        Create, Play, Death
    }
    public STATE myState = STATE.Create;
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
    public override void PlayerMove()
    {
        myAnim.speed = myMoveSpeed;

        float x = 0.0f;
        float z = 0.0f;
        if (myCameras.myCameraState == SpringArms.ViewState.UI) targetDir = Vector2.zero; //UI ���¿��� �������̰�
        else
        {
            targetDir.x = GetMoveRaw().x;
            targetDir.y = GetMoveRaw().y;
            x = Mathf.Lerp(myAnim.GetFloat("x"), targetDir.x, Time.deltaTime * smoothMoveSpeed);
            z = Mathf.Lerp(myAnim.GetFloat("z"), targetDir.y, Time.deltaTime * smoothMoveSpeed);

            myMoveSpeed = (Input.GetKey(KeyCode.LeftShift) && !IsWall) ? 1.5f : myStat.MoveSpeed; // �� �浹�ÿ� ���� off

            //x, z���� 0�� ������ 0���� ����
            if (Mathf.Epsilon - animOffset < x && x < Mathf.Epsilon + animOffset) x = 0.0f;
            if (Mathf.Epsilon - animOffset < z && z < Mathf.Epsilon + animOffset) z = 0.0f;
            myAnim.SetFloat("x", x);
            myAnim.SetFloat("z", z);

            HandlePlayerMovement();
        }

    }
    private void OnCollisionStay(Collision collision) // �� �浹�ÿ� ���� off
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            //myMoveSpeed = 0.5f; ���浹 �̵��ӵ��� ���߿� ����
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
        // �׳� LayerMask.NameToLayer("Enemy"))�� �ϸ� ���̾ �����Ѱ� ���õȴ�
        foreach (Collider col in list)
        {
            Debug.Log(col);
            col.GetComponent<Monster>().GetKick(myCameras.myRoot.transform.forward, KickStrength);
        }
    }
    public void KickCheck(bool v)
    {
        KickPoint.SetActive(v);
    }
    public override Animator ReturnAnim()
    {
        return myAnim;
    }
}
