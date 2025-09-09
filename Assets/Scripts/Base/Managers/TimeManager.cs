
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour, iTimeFunctionality, EventListener<ItemEffectStatesEvent>
{
    private static TimeManager _inst = null;
    public static TimeManager Inst => _inst;

    [Header("시간 속도 설정")]
    [SerializeField] private float pauseTime = 0.01f;
    [SerializeField] private float normalTime = 1f;

    [Range(0f, 1f)]
    private float gameTimeScale = 1f;
    [Tooltip("게임 재개 시 복귀할 시간 스케일")]
    private float prevTimeScale = 1f;  // 이전 시간 스케일 저장
    public float GameTimeScale => gameTimeScale;
    
    private const float BaseFixedTime = 0.02f;
    private bool isSlowing = false;
    public bool IsSlowing => isSlowing;
    private GameState myGameState = GameState.Play;
    private float remainingDuration = 0f;
    private Coroutine timeStopCoroutine;

    private void Awake()
    {
        if (_inst == null)
        {
            _inst = this;
            DontDestroyOnLoad(gameObject);
            this.EventStartingListening<ItemEffectStatesEvent>(); // GameEventManager를 통해 구독
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_inst == this)
        {
            this.EventStopListening<ItemEffectStatesEvent>(); // GameEventManager를 통해 구독 해지
        }
    }

    private void Update()
    {
        Time.timeScale = GameTimeScale;
    }

    public void Pause()
    {
        myGameState = GameState.Pause;
        prevTimeScale = gameTimeScale;
        gameTimeScale = pauseTime;
    }

    public void UnPause()
    {
        myGameState = GameState.Play;
        gameTimeScale = prevTimeScale;
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
        gameTimeScale = targetScale;
        remainingDuration = duration;

        while (remainingDuration > 0f)
        {
            if (myGameState == GameState.Play)
            {
                remainingDuration -= Time.unscaledDeltaTime;
            }
            yield return null;
        }

        // 시간이 다 되면 원래 속도로 복귀
        gameTimeScale = normalTime;
        isSlowing = false;
    }
}
