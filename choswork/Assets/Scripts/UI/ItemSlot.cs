using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemSlot : MonoBehaviour
{
    public int itemCount; // »πµÊ«— æ∆¿Ã≈€¿« ∞≥ºˆ
    public GameObject ItemImage;
    public Transform mySlotMask;
    [SerializeField] private Text text_Count;
    [SerializeField] private GameObject go_CountImage;
    [SerializeField] private Item myItem;
    public void GetItem(Item _item)
    {
        myItem = _item;
        ItemImage = Instantiate(_item.itemImage, mySlotMask);
        ItemImage.transform.SetAsLastSibling();
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
            Debug.Log(myItem + " ¡¶∞≈µ ");
            myItem = null;
        }
    }
    public void SwitchSlot(ItemSlot slot)
    {
        ItemImage.transform.SetParent(slot.mySlotMask);
        ItemImage.transform.SetAsLastSibling();
        slot.ItemImage = ItemImage;
        slot.myItem = myItem;
        ItemImage.transform.localPosition = Vector3.zero;

        ItemImage = null;
        myItem = null;
    }
}
