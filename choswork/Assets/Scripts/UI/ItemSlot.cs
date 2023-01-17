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
    [SerializeField] private GameObject ItemImage;
    [SerializeField] private Item myItem;
    public void GetItem(Item _item)
    {
        myItem = _item;
        ItemImage = Instantiate(_item.itemImage, mySlotMask);
        ItemImage.transform.SetAsLastSibling();
        IsItem = true;
    }
    public Item GetItemValue()
    {
        return myItem;
    }
    public void DestroyItem()
    {
        if(myItem != null)
        {
            Destroy(ItemImage);
            IsItem = false;
        }
    }
}
