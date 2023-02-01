using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : InputManager, ItemEvent
{
    [SerializeField] private GameObject InventoryBase;
    [SerializeField] private GameObject SlotParent;
    [SerializeField] private bool isInventory = false;
    [SerializeField] private ItemSlot[] mySlots;
    [SerializeField] private Transform myTargetObj;
    private Dictionary<Item, List<ItemSlot>> itemTypeToSlotListMap = new Dictionary<Item, List<ItemSlot>>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (var slot in mySlots)
        {
            Item item = slot.GetItemValue();
            if (item != null)
            {
                if (!itemTypeToSlotListMap.ContainsKey(item))
                    itemTypeToSlotListMap[item] = new List<ItemSlot>();

                itemTypeToSlotListMap[item].Add(slot);
            }
        }
        InventoryBase.SetActive(false);
        mySlots = SlotParent?.GetComponentsInChildren<ItemSlot>();
    }
    public override void ToggleInventory()
    {
        isInventory = !isInventory;
        InventoryBase.SetActive(isInventory);
    }
    public void AcquireItem(Item _item)
    {
        foreach (var slot in mySlots)
        {
            Item curitem = slot.GetItemValue();
            if (curitem == null) // 슬롯이 비어있다
            {
                slot.GetItem(_item); // 아이템 획득
                if (!itemTypeToSlotListMap.ContainsKey(_item))
                {
                    itemTypeToSlotListMap[_item] = new List<ItemSlot>() { slot };
                    Debug.Log(_item + "슬롯 생성");
                }
                else
                {
                    itemTypeToSlotListMap[_item].Add(slot);
                    Debug.Log(_item + "슬롯에 추가");
                }
                return;
            }
        }
        
    }
    public ItemSlot FindSlot(bool IsFirst, bool IsItem)
    {
        if (IsFirst)
        {
            return mySlots.FirstOrDefault(s => (IsItem == (s.GetItemValue() != null)));
        }
        else
        {
            return mySlots.LastOrDefault(s => (IsItem == (s.GetItemValue() != null)));
        }
    }
    public void DestroyItem(Item _item)
    {
        if (itemTypeToSlotListMap.TryGetValue(_item, out var itemSlotList))
        {
            var itemSlot = itemSlotList.FindLast(s => s.GetItemValue() == _item);
            if (itemSlot != null)
            {
                itemSlot.DestroyItem();
                myTargetObj?.GetComponent<ItemEvent>()?.SetItemEvent();
                itemTypeToSlotListMap[_item].Remove(itemSlot);
            }
        }
        else Debug.Log("아이템이 없습니다.");
        SortItems();
    }
    public void SortItems()
    {
        var firstEmptySlot = FindSlot(true, false);
        int emptySlotIndex = Array.FindIndex(mySlots, slot => slot.GetItemValue() == null);
        //int emptySlotIndex = Array.IndexOf(mySlots, firstEmptySlot);
        for (int i = emptySlotIndex; i < mySlots.Length; i++)
        {
            if (mySlots[i].GetItemValue() != null)
            {
                mySlots[i].SwitchSlot(firstEmptySlot);
                firstEmptySlot = mySlots[i];
            }
        }
    }
    public void SetItemEvent()
    {

    }
    public void SetItemTargetObj(Transform target)
    {
        myTargetObj = target;
    }
}
