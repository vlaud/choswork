using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PickUpController : ObjectGrabbable
{
    public GameObject GetInventory;
    private ItemPickUp myItem;
    private void Start()
    {
        myInventory = GetInventory;
    }
    public void CanPickUp()
    {
        myItem = GetComponent<ItemPickUp>();
        Debug.Log(myItem.item.itemName + " ȹ�� �߽��ϴ�.");  // �κ��丮 �ֱ�
        Destroy(gameObject);
        SetItemInfoAppear(false);
        myInventory?.transform.GetComponent<Inventory>().AcquireItem(myItem.item);
    }
}
