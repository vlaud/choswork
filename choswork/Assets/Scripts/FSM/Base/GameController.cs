using System.Collections.Generic;
using UnityEngine;

public enum Locations { SweetHome = 0, Library, LectureRoom, PCRoom, Pub };

public class GameController : MonoBehaviour
{
    [SerializeField]
    private string[] arrayStudents; // Student���� �̸� �迭
    [SerializeField]
    private GameObject studentPrefab;  // Student Ÿ���� ������

    [SerializeField]
    private string[] arrayUnemployeds; // Unemployed���� �̸� �迭
    [SerializeField]
    private GameObject unemployedPrefab;   // Unemployed Ÿ���� ������

    // ��� ��� ���� ��� ������Ʈ ����Ʈ
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

        // ������Ʈ �����ͺ��̽� �ʱ�ȭ �� ��� ������Ʈ ���
        EntityDatabase.Instance.Setup();
        for (int i = 0; i < entitys.Count; ++i)
        {
            EntityDatabase.Instance.RegisterEntity(entitys[i]);
        }

        // �޽��� ������ �ʱ�ȭ
        MessageDispatcher.Instance.Setup();
    }

    private void Update()
    {
        if (IsGameStop == true) return;

        // ���� �߼۵Ǿ�� �ϴ� �޽��� ����
        MessageDispatcher.Instance.DispatchDelayedMessages();

        // ��� ������Ʈ�� Updated()�� ȣ���� ������Ʈ ����
        for (int i = 0; i < entitys.Count; ++i)
        {
            entitys[i].Updated();
        }
    }

    public static void Stop(BaseGameEntity entity)
    {
        IsGameStop = true;

        entity.PrintText("�����մϴ�.");
    }
}

