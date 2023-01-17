using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : InputManager
{
    [SerializeField] private GameObject InventoryBase;
    [SerializeField] private GameObject SlotParent;
    [SerializeField] private bool isInventory = false;
    [SerializeField] private ItemSlot[] mySlots;
    [SerializeField] private int ActiveSlots;
    // Start is called before the first frame update
    void Start()
    {
        InventoryBase.SetActive(false);
        mySlots = SlotParent?.GetComponentsInChildren<ItemSlot>();
        ActiveSlots = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void ToggleInventory()
    {
        isInventory = !isInventory;
        InventoryBase.SetActive(isInventory);
    }
    public void AcquireItem(Item _item)
    {
        for (int i = 0; i < mySlots.Length; ++i)
        {
            if (!mySlots[i].IsItem)
            {
                mySlots[i].GetItem(_item);
                ActiveSlots++;
                return;
            }
        }
    }
    public ItemSlot FindLastSlotNotEmpty()
    {
        int slotNum;
        for (int i = 0; i < mySlots.Length; ++i)
        {
            slotNum = mySlots.Length - i - 1;

            if (mySlots[slotNum].IsItem)
            {
                return mySlots[slotNum];
            }
        }
        return null;
    }
    public ItemSlot FindItem(Item _item)
    {
        int slotNum;
        for (int i = 0; i < mySlots.Length; ++i)
        {
            slotNum = mySlots.Length - i - 1;
            if (!mySlots[slotNum].IsItem) continue;

            if (mySlots[slotNum].GetItemValue() == _item)
            {
                return mySlots[slotNum];
            }
        }
        return null;
    }
    public void DestroyItem(Item _item)
    {
        FindItem(_item).DestroyItem();
        ActiveSlots--;
    }
}
