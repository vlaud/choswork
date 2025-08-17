
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour, iTimeFunctionality, EventListener<ItemEffectStatesEvent>
{
    private static TimeManager _inst = null;
    public static TimeManager Inst => _inst;

    [Header("시간 속도 설정")]
    [SerializeField] private float pauseTime = 0.01f;

    [Range(0f, 1f)]
    private float gameTimeScale = 1f;
    public float GameTimeScale => gameTimeScale;
    
    [ReadOnly]
    public float GameFixedTimeScale = 0.02f;

    private const float BaseFixedTime = 0.02f;
    private bool isSlowing = false;
    public bool IsSlowing => isSlowing;
    private Coroutine timeStopCoroutine;

    private void Awake()
    {
        if (_inst == null)
        {
            _inst = this;
            DontDestroyOnLoad(gameObject);
            this.EventStartingListening<ItemEffectStatesEvent>(); // GameEventManager를 통해 구독
            //Physics.simulationMode = SimulationMode.Script;
        }
        else
        {
            Destroy(gameObject);
            //Physics.simulationMode = SimulationMode.FixedUpdate;
        }
    }

    private void OnDestroy()
    {
        if (_inst == this)
        {
            this.EventStopListening<ItemEffectStatesEvent>(); // GameEventManager를 통해 구독 해지
        }
    }

    private float timer;

    private void Update()
    {
        Time.timeScale = GameTimeScale;
        //GameFixedTimeScale = Time.timeScale * BaseFixedTime;

        // // 수동 물리 업데이트
        // if (GameTimeScale > 0)
        // {
        //     timer += Time.deltaTime;
        //     while (timer >= GameFixedTimeScale)
        //     {
        //         timer -= GameFixedTimeScale;
        //         Physics.SyncTransforms();
        //         Physics.Simulate(GameFixedTimeScale);
        //     }
        // }
    }

    public void Pause()
    {
        isSlowing = false;
        StopAllCoroutines();
        gameTimeScale = pauseTime;
    }

    public void UnPause(float previousTimeScale)
    {
        gameTimeScale = previousTimeScale;
    }

    public void StartSlowMotion(float targetScale)
    {
        if (isSlowing) return;
        isSlowing = true;
        this.StartOrRestartCoroutine(ref timeStopCoroutine, ChangeTimeScale(targetScale));
    }

    public void StopSlowMotion()
    {
        if (!isSlowing) return;
        isSlowing = false;
        this.StartOrRestartCoroutine(ref timeStopCoroutine, ChangeTimeScale(1.0f));
    }

    // EventListener<TimeStopEvent> 인터페이스 구현
    public void OnEvent(ItemEffectStatesEvent e)
    {
        if (e.itemEffectTelegramEvent.itemEffectEventsType != ItemEffectEventsType.TimeSlow) return;
        if (isSlowing) return; // 이미 슬로우 모션 중이면 무시

        isSlowing = true;
        this.StartOrRestartCoroutine(ref timeStopCoroutine, ProcessTimeStop(e.itemEffectTelegramEvent.targetScale, e.itemEffectTelegramEvent.duration));
    }
    
    private IEnumerator ProcessTimeStop(float targetScale, float duration)
    {
        yield return StartCoroutine(ChangeTimeScale(targetScale));

        // Time.timeScale에 영향을 받지 않는 실제 시간으로 대기
        yield return new WaitForSecondsRealtime(duration);

        // 시간이 다 되면 원래 속도로 복귀
        StopSlowMotion();
    }

    private IEnumerator ChangeTimeScale(float targetScale)
    {
        // 1. 문제가 될 수 있는 (거의 멈춰있는) Rigidbody만 kinematic으로 전환
        List<Rigidbody> settledRigidbodies = new List<Rigidbody>();
        var allRagdollActions = FindObjectsByType<RagDollAction>(FindObjectsSortMode.None);

        foreach (var action in allRagdollActions)
        {
            if (action.IsRagdolled())
            {
                var rigidbodies = action.GetRagdollRigidbodies();
                foreach (var rb in rigidbodies)
                {
                    if (rb != null && !rb.isKinematic && rb.linearVelocity.magnitude < 0.1f)
                    {
                        rb.isKinematic = true;
                        settledRigidbodies.Add(rb);
                    }
                }
            }
        }

        // 2. Time.timeScale 변경
        gameTimeScale = targetScale;

        // 3. 한 물리 프레임 대기
        yield return new WaitForFixedUpdate();

        // 4. Kinematic 상태를 원상 복구
        foreach (var rb in settledRigidbodies)
        {
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.WakeUp();
            }
        }
    }
}
