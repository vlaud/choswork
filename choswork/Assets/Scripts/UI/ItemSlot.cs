using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemSlot : MonoBehaviour
{
    public int itemCount; // »πµÊ«— æ∆¿Ã≈€¿« ∞≥ºˆ
    public bool IsItem = false;
    public Transform mySlotMask;
    [SerializeField] private Text text_Count;
    [SerializeField] private GameObject go_CountImage;
   
    public void GetItem(Item _item)
    {
        GameObject obj = Instantiate(_item.itemImage, mySlotMask);
        obj.transform.SetAsLastSibling();
        IsItem = true;
    }
}
