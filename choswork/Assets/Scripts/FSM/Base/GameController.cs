using System.Collections.Generic;
using UnityEngine;

public enum Locations { SweetHome = 0, Library, LectureRoom, PCRoom, Pub };

public class GameController : MonoBehaviour
{
    [SerializeField]
    private string[] arrayStudents; // Student들의 이름 배열
    [SerializeField]
    private GameObject studentPrefab;  // Student 타입의 프리팹

    [SerializeField]
    private string[] arrayUnemployeds; // Unemployed들의 이름 배열
    [SerializeField]
    private GameObject unemployedPrefab;   // Unemployed 타입의 프리팹

    // 재생 제어를 위한 모든 에이전트 리스트
    private List<BaseGameEntity> entitys;

    public static bool IsGameStop { set; get; } = false;

    private void Awake()
    {
        var baseGameEntities = FindObjectsOfType(typeof(BaseGameEntity)) as BaseGameEntity[];
        entitys = new List<BaseGameEntity>();

        for (int i = 0; i < baseGameEntities.Length; ++i)
        {
            baseGameEntities[i].Setup(baseGameEntities[i].ToString());
            entitys.Add(baseGameEntities[i]);
        }

        // 에이전트 데이터베이스 초기화 및 모든 에이전트 등록
        EntityDatabase.Instance.Setup();
        for (int i = 0; i < entitys.Count; ++i)
        {
            EntityDatabase.Instance.RegisterEntity(entitys[i]);
        }

        // 메시지 관리자 초기화
        MessageDispatcher.Instance.Setup();
    }

    private void Update()
    {
        if (IsGameStop == true) return;

        // 지연 발송되어야 하는 메시지 관리
        MessageDispatcher.Instance.DispatchDelayedMessages();

        // 모든 에이전트의 Updated()를 호출해 에이전트 구동
        for (int i = 0; i < entitys.Count; ++i)
        {
            entitys[i].Updated();
        }
    }

    public static void Stop(BaseGameEntity entity)
    {
        IsGameStop = true;

        entity.PrintText("종료합니다.");
    }
}

