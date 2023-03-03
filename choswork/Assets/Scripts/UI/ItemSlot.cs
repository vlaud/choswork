using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public int itemCount = 0; // »πµÊ«— æ∆¿Ã≈€¿« ∞≥ºˆ
    public GameObject ItemImage;
    public Transform mySlotMask;
    [SerializeField] private TMPro.TMP_Text text_Count;
    [SerializeField] private GameObject go_CountImage;
    [SerializeField] private Item myItem;
    public void GetItem(Item _item, int _count = 1)
    {
        myItem = _item;
        itemCount += _count;
        if (ItemImage == null)
        {
            ItemImage = Instantiate(_item.itemImage, mySlotMask);
            ItemImage.transform.SetAsLastSibling();
        }
       
        if (myItem.itemType != Item.ItemType.Equipment)
        {
            go_CountImage.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            text_Count.text = "0";
            go_CountImage.SetActive(false);
        }

    }
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    // «ÿ¥Á ΩΩ∑‘ «œ≥™ ªË¡¶
    private void ClearSlot()
    {
        myItem = null;
        itemCount = 0;

        text_Count.text = "0";
        go_CountImage.SetActive(false);
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
