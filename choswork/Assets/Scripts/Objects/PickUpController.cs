using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PickUpController : ObjectGrabbable
{
    public void CanPickUp()
    {
        Debug.Log(GetComponent<ItemPickUp>().item.itemName + " ȹ�� �߽��ϴ�.");  // �κ��丮 �ֱ�
        Destroy(gameObject);
        SetItemInfoAppear(false);
    }
}
