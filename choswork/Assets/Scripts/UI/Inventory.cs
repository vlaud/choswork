using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public override void ToggleInventory()
    {
        isInventory = !isInventory;
        InventoryBase.SetActive(isInventory);
    }
    public void AcquireItem(Item _item)
    {
        for (int i = 0; i < mySlots.Length; ++i)
        {
            if (mySlots[i].GetItemValue() == null)
            {
                mySlots[i].GetItem(_item);
                ActiveSlots++;
                return;
            }
        }
    }
    public int GetSlotIndex(ItemSlot slot)
    {
        return Array.IndexOf(mySlots, slot);
    }
    public ItemSlot FindSlot(bool IsFirst, bool IsItem)
    {
        int slotNum;
        for (int i = 0; i < mySlots.Length; ++i)
        {
            if(!IsFirst)
                slotNum = mySlots.Length - i - 1;
            else
                slotNum = i;

            if (IsItem == (mySlots[slotNum].GetItemValue() != null)) // IsItem: ¾ÆÀÌÅÛ Á¸Àç ¿©ºÎ
            {
                return mySlots[slotNum];
            }
        }
        return null;
    }
    public ItemSlot FindSlotbyItem(Item _item)
    {
        int slotNum;
        for (int i = 0; i < mySlots.Length; ++i)
        {
            slotNum = mySlots.Length - i - 1;
            if (mySlots[slotNum].GetItemValue() == null) continue;

            if (mySlots[slotNum].GetItemValue() == _item)
            {
                return mySlots[slotNum];
            }
        }
        return null;
    }
    public void DestroyItem(Item _item)
    {
        FindSlotbyItem(_item)?.DestroyItem();
        ActiveSlots--;
        StartCoroutine(SortItems());
    }
    IEnumerator SortItems()
    {
        ItemSlot firstEmptySlot = FindSlot(true, false);
        ItemSlot lastNotEmptySlot = FindSlot(false, true);
        int index = GetSlotIndex(FindSlot(true, false)) + 1;

        while (GetSlotIndex(firstEmptySlot) < GetSlotIndex(lastNotEmptySlot))
        {
            if (mySlots[index].GetItemValue() != null)
            {
                Debug.Log("ºó ½½·Ô: " + GetSlotIndex(firstEmptySlot));
                Debug.Log("¹Ù²ã¾ßÇÒ ½½·Ô: " + index);
                mySlots[index].SwitchSlot(firstEmptySlot);
            }
            else index++;
            
            firstEmptySlot = FindSlot(true, false);
            lastNotEmptySlot = FindSlot(false, true);
            Debug.Log("¸¶Áö¸· ¾ÆÀÌÅÛ ½½·Ô: " + GetSlotIndex(lastNotEmptySlot));
            yield return null;
        }
    }
}
