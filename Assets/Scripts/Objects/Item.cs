using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/New item")]
public class Item : ScriptableObject
{
    public enum ItemType  // ������ ����
    {
        Equipment,
        Used,
        Ingredient,
        ETC,
    }

    public int[] itemCount; // ������ ����
    public string itemName; // �������� �̸�
    public ItemType itemType; // ������ ����
    public GameObject itemImage; // �������� �̹���(�κ� �丮 �ȿ��� ���)
    public Texture itemTex;
    public GameObject itemPrefab;  // �������� ������ (������ ������ ���������� ��)

    public string weaponType;  // ���� ����
}
