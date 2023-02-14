using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PickUpController : ObjectGrabbable
{
    private ItemPickUp myItem;
    private void Start()
    {
        myInventory = GameManagement.Inst.myInventory.gameObject;
        for (int i = 0; i < GameManagement.Inst.myMonsters.Length; ++i)
            hearings.Add(GameManagement.Inst.myMonsters[i].HearingSound);
        SetActionText();
    }
    public void CanPickUp()
    {
        myItem = GetComponent<ItemPickUp>();
        Debug.Log(myItem.item.itemName + " 획득 했습니다.");  // 인벤토리 넣기
        Destroy(gameObject);
        SetItemInfoAppear(false);
        myInventory?.GetComponent<Inventory>().AcquireItem(myItem.item);
    }
}
