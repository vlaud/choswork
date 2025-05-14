using UnityEngine;

public enum PlayerStates { Play = 0, Pause, Death, Global }

public class Player : BattleSystem, iSubscription, EventListener<PlayerStatesEvent>, EventListener<GameStatesEvent>
{
    Vector2 targetDir = Vector2.zero;
    [Header("�÷��̾� �⺻ ����")]
    public float smoothMoveSpeed = 10.0f;
    public float myMoveSpeed = 1.0f;
    public float KickStrength = 10000.0f;
    public SpringArms myCameras;
    public GameObject KickPoint; // ���� ������ ȿ�� ��ġ
    public Transform KickTransform; // ������ �Ҹ� ��ġ
    public bool IsWall = false;
    [SerializeField] private bool isGhost = false;
    public HPBar myHPBar;
    private float animOffset = 0.04f;
    private GameManagement myGamemanager;
    public Transform myHips;
    [Header("ī�޶� ����Ʈ ����")]
    public CameraShake camShake;
    [SerializeField] private CameraSet? curCamset;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private float shake_duration;
    [SerializeField] private float shake_magnitude;
    [SerializeField] private bool IsPosPerlin;
    [SerializeField] private bool IsRotPerlin;
    [Header("����Ʈ ����")]
    [SerializeField] private GameObject bloodEffect_origin;
    [SerializeField] private GameObject bloodEffect;
    [SerializeField] private Transform bloodPos;
    [SerializeField] private GameObject timeStopEffect;
    [Header("������ ����")]
    public Item timeStopItem;
    public bool IsTimtStopAvailable = false;
    public enum STATE
    {
        Create, Play, Pause, Death
    }
    [Header("���� ����")]
    public STATE myState = STATE.Create;

    private PlayerStates currentState;

    public PlayerStates CurrentState => currentState;

    void ChangeState(STATE s)
    {
        if (myState == s) return;
        myState = s;

        switch (myState)
        {
            case STATE.Play:
                break;
            case STATE.Pause:
                break;
            case STATE.Death:
                DeadAction();
                break;
        }
    }

    public void DeadAction()
    {
        StopAllCoroutines();
        GameManagement.Inst.GameOver();
        myAnim.SetTrigger("Dead");
        foreach (IBattle ib in myAttackers)
        {
            ib.DeadMessage(transform);
        }
        Unsubscribe();
    }

    void StateProcess()
    {
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
        playerCamera = curCamset?.realCam;
        camShake = playerCamera?.GetComponent<CameraShake>();
        myGamemanager = GameManagement.Inst;
        myMoveSpeed = myStat.MoveSpeed;
        KickPoint.SetActive(false);
        transform.position = myGamemanager.myMapManager.PlayerStart.position;
        transform.rotation = myGamemanager.myMapManager.PlayerStart.rotation;
        myStat.changeHP = (float v) => myHPBar.GetValue = v;
        //myInventory = GameManagement.Inst.myInventory;
        Subscribe();
        ChangeState(STATE.Play);
    }

    // Update is called once per frame
    void Update()
    {
        StateProcess();
        curCamset = myCameras.GetMyCamera();
        playerCamera = curCamset?.realCam;
        camShake = playerCamera?.GetComponent<CameraShake>();
        if (!myAnim.GetBool("IsHit") && bloodEffect != null && bloodEffect.activeSelf == true)
            bloodEffect.SetActive(false);
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void Subscribe()
    {
        this.EventStartingListening<GameStatesEvent>();
        this.EventStartingListening<PlayerStatesEvent>();
    }

    public void Unsubscribe()
    {
        this.EventStopListening<GameStatesEvent>();
        this.EventStopListening<PlayerStatesEvent>();
    }

    public void OnEvent(GameStatesEvent eventType)
    {
        switch (eventType.gameEventType)
        {
            case GameEventType.Pause:
                ChangeState(STATE.Pause);
                break;
            case GameEventType.UnPause:
                ChangeState(STATE.Play);
                break;
        }
    }

    public void OnEvent(PlayerStatesEvent eventType)
    {
        switch (eventType.playerEventType)
        {
            case PlayerEventType.TimeStop:
                TimeStop();
                break;
            case PlayerEventType.PlayerKick:
                PlayKick();
                break;
            case PlayerEventType.Dash:
                Dash();
                break;
            case PlayerEventType.NormalSpeed:
                NormalSpeed();
                break;
        }
    }

    void PlayKick()
    {
        if (myAnim.GetBool("IsKicking")) return;

        myAnim.SetTrigger("Kick");
    }

    void NormalSpeed()
    {
        myMoveSpeed = myStat.MoveSpeed;
        myAnim.speed = myMoveSpeed;
    }

    void Dash()
    {
        myMoveSpeed = (!IsWall) ? 1.5f : myStat.MoveSpeed; // �� �浹�ÿ� ���� off
        myAnim.speed = myMoveSpeed;
    }

    public void PlayerMove()
    {
        float x, z;
        if (myCameras?.myCameraState == ViewState.UI) targetDir = Vector2.zero; //UI ���¿��� �������̰�
        else
        {
            targetDir.x = Input.GetAxisRaw("Horizontal");
            targetDir.y = Input.GetAxisRaw("Vertical");
        }

        x = Mathf.Lerp(myAnim.GetFloat("x"), targetDir.x, Time.unscaledDeltaTime * smoothMoveSpeed);
        z = Mathf.Lerp(myAnim.GetFloat("z"), targetDir.y, Time.unscaledDeltaTime * smoothMoveSpeed);

        //x, z���� 0�� ������ 0���� ����
        if (Mathf.Epsilon - animOffset < x && x < Mathf.Epsilon + animOffset) x = 0.0f;
        if (Mathf.Epsilon - animOffset < z && z < Mathf.Epsilon + animOffset) z = 0.0f;
        myAnim.SetFloat("x", x);
        myAnim.SetFloat("z", z);
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

    public void TimeStop()
    {
        if (isGhost) return;
        if (GameManagement.Inst.GetIsBulletTime()) return;

        IsTimtStopAvailable = GameManagement.Inst.myInventory.IsItemExist(timeStopItem);

        if (IsTimtStopAvailable)
        {
            Debug.Log("TimeStop");
            GameManagement.Inst.SetBulletTime(0.3f, 5f);
            GameManagement.Inst.myInventory.DestroyItem(timeStopItem);
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
        // �׳� LayerMask.NameToLayer("Enemy"))�� �ϸ� ���̾ �����Ѱ� ���õȴ�
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
    public Animator ReturnAnim()
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

    public void SetGhost(bool v)
    {
        isGhost = v;
    }
}
