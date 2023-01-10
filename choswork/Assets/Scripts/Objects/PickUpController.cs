using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PickUpController : ObjectGrabbable
{
    public void CanPickUp()
    {
        Debug.Log(GetComponent<ItemPickUp>().item.itemName + " 획득 했습니다.");  // 인벤토리 넣기
        Destroy(gameObject);
        SetItemInfoAppear(false);
    }
}
