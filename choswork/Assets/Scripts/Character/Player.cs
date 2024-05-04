using UnityEngine;

public class Player : BattleSystem
{
    Vector2 targetDir = Vector2.zero;
    [Header("플레이어 기본 설정")]
    public float smoothMoveSpeed = 10.0f;
    public float myMoveSpeed = 1.0f;
    public float KickStrength = 10000.0f;
    public SpringArms myCameras;
    public GameObject KickPoint; // 실제 발차기 효과 위치
    public Transform KickTransform; // 발차기 소리 위치
    public bool IsWall = false;
    public HPBar myHPBar;
    private float animOffset = 0.04f;
    private GameManagement myGamemanager;
    public Transform myHips;
    [Header("카메라 이펙트 설정")]
    public CameraShake camShake;
    [SerializeField] private CameraSet? curCamset;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private float shake_duration;
    [SerializeField] private float shake_magnitude;
    [SerializeField] private bool IsPosPerlin;
    [SerializeField] private bool IsRotPerlin;
    [Header("이펙트 설정")]
    [SerializeField] private GameObject bloodEffect_origin;
    [SerializeField] private GameObject bloodEffect;
    [SerializeField] private Transform bloodPos;
    [SerializeField] private GameObject timeStopEffect;
    [Header("아이템 설정")]
    public Item timeStopItem;
    public bool IsTimtStopAvailable = false;
    public enum STATE
    {
        Create, Play, Pause, Death
    }
    [Header("상태 설정")]
    public STATE myState = STATE.Create;

    void ChangeState(STATE s)
    {
        if (myState == s) return;
        myState = s;

        switch (myState)
        {
            case STATE.Play:
                GameManagement.Inst.UnPauseGame();
                break;
            case STATE.Pause:
                GameManagement.Inst.PauseGame();
                break;
            case STATE.Death:
                StopAllCoroutines();
                GameManagement.Inst.GameOver();
                myAnim.SetTrigger("Dead");
                foreach (IBattle ib in myAttackers)
                {
                    ib.DeadMessage(transform);
                }
                break;
        }
    }
    void StateProcess()
    {
        HandleOtherInput();
        switch (myState)
        {
            case STATE.Play:
                PlayerMove();
                break;
            case STATE.Pause:
                break;
            case STATE.Death:
                break;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        curCamset = myCameras.GetMyCamera();
        playerCamera = curCamset?.myCam;
        camShake = playerCamera?.GetComponent<CameraShake>();
        myGamemanager = GameManagement.Inst;
        myMoveSpeed = myStat.MoveSpeed;
        KickPoint.SetActive(false);
        transform.position = myGamemanager.myMapManager.PlayerStart.position;
        transform.rotation = myGamemanager.myMapManager.PlayerStart.rotation;
        myStat.changeHP = (float v) => myHPBar.GetValue = v;
        myInventory = GameManagement.Inst.myInventory;
        ChangeState(STATE.Play);
    }

    // Update is called once per frame
    void Update()
    {
        StateProcess();
        curCamset = myCameras.GetMyCamera();
        playerCamera = curCamset?.myCam;
        camShake = playerCamera?.GetComponent<CameraShake>();
        if(!myAnim.GetBool("IsHit") && bloodEffect != null && bloodEffect.activeSelf == true) 
            bloodEffect.SetActive(false);
    }
    public override void PlayerMove()
    {
        myAnim.speed = myMoveSpeed;

        float x, z;
        if (myCameras?.myCameraState == SpringArms.ViewState.UI) targetDir = Vector2.zero; //UI 상태에선 못움직이게
        else
        {
            targetDir.x = GetMoveRaw().x;
            targetDir.y = GetMoveRaw().y;
        }

        x = Mathf.Lerp(myAnim.GetFloat("x"), targetDir.x, Time.unscaledDeltaTime * smoothMoveSpeed);
        z = Mathf.Lerp(myAnim.GetFloat("z"), targetDir.y, Time.unscaledDeltaTime * smoothMoveSpeed);

        myMoveSpeed = (IsDashKeyPressed() && !IsWall) ? 1.5f : myStat.MoveSpeed; // 벽 충돌시엔 질주 off

        //x, z값이 0에 가까우면 0으로 고정
        if (Mathf.Epsilon - animOffset < x && x < Mathf.Epsilon + animOffset) x = 0.0f;
        if (Mathf.Epsilon - animOffset < z && z < Mathf.Epsilon + animOffset) z = 0.0f;
        myAnim.SetFloat("x", x);
        myAnim.SetFloat("z", z);

        HandlePlayerMovement();
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
    public override void TimeStop()
    {
        if (GameManagement.Inst.GetIsBulletTime()) return;

        IsTimtStopAvailable = true;
        if (!myInventory.IsItemExist(timeStopItem)) IsTimtStopAvailable = false;
        
        if (IsTimtStopAvailable)
        {
            GameManagement.Inst.SetBulletTime(0.3f, 5f);
            myInventory.DestroyItem(timeStopItem);
            if (!myInventory.IsItemExist(timeStopItem)) IsTimtStopAvailable = false;
        }
    }
    public void TimeStopCheck(bool v)
    {
        if (timeStopEffect == null)
            timeStopEffect = Instantiate(Resources.Load("Prefabs/forcefield"), transform) as GameObject;
        
        timeStopEffect.SetActive(v);
        timeStopEffect.transform.localScale = new Vector3(3f, 3f, 3f);
    }
    public void KickTarget()
    {
        Collider[] list = Physics.OverlapSphere(KickPoint.transform.position, 0.2f, 1 << LayerMask.NameToLayer("Enemy"));
        // 그냥 LayerMask.NameToLayer("Enemy"))을 하면 레이어가 엉뚱한게 선택된다
        foreach (Collider col in list)
        {
            Debug.Log(col);
            camShake?.OnShakeCamera(shake_duration, shake_magnitude, IsPosPerlin);
            camShake?.OnRotateCamera(shake_duration, shake_magnitude, IsRotPerlin);
            col.GetComponent<RagDollAction>().GetKick(myCameras.myRoot.transform.forward, KickStrength);
        }
    }
    public void KickCheck(bool v)
    {
        KickPoint.SetActive(v);
    }
    public void BleedCheck(bool v)
    {
        if (bloodEffect == null)
            bloodEffect = Instantiate(bloodEffect_origin, bloodPos);

        bloodEffect.SetActive(v);
    }
    public override Animator ReturnAnim()
    {
        return myAnim;
    }
    public override void OnDamage(float dmg)
    {
        camShake?.OnShakeCamera(shake_duration, shake_magnitude, IsPosPerlin);
        camShake?.OnRotateCamera(shake_duration, shake_magnitude, IsRotPerlin);
        myStat.HP -= dmg;
        if (Mathf.Approximately(myStat.HP, 0.0f))
        {
            ChangeState(STATE.Death);
            return;
        }
        myAnim.SetTrigger("Damage");
    }
    public override void ToggleEscapeEvent()
    {
        if (myState == STATE.Death) return;

        if (myState != STATE.Pause)
            ChangeState(STATE.Pause);
        else
            ChangeState(STATE.Play);
    }
}
