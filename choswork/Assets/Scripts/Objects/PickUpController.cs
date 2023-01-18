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
