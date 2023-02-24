using UnityEngine;

public class PickUpController : ObjectGrabbable
{
    private ItemPickUp myItem;
    private void Start()
    {
        myInventory = GameManagement.Inst.myInventory.gameObject;
        for (int i = 0; i < GameManagement.Inst.myMonsters.Length; ++i)
            hearings.Add(GameManagement.Inst.myMonsters[i].GetComponent<AIAction>().HearingSound);
        SetActionText();
    }
    public void CanPickUp()
    {
        myItem = GetComponent<ItemPickUp>();
        Debug.Log(myItem.item.itemName + " ȹ�� �߽��ϴ�.");  // �κ��丮 �ֱ�
        Destroy(gameObject);
        SetItemInfoAppear(false);
        myInventory?.GetComponent<Inventory>().AcquireItem(myItem.item);
    }
}
