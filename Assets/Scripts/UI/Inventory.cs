using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour, ItemTargeting, EventListener<CameraStatesEvent>, EventListener<GameStatesEvent>, iSubscription
{
    [SerializeField] private GameObject InventoryBase;
    [SerializeField] private GameObject SlotParent;
    [SerializeField] private bool isInventory = false;
    [SerializeField] private ItemSlot[] mySlots;
    [SerializeField] private Transform myTargetObj;
    private Dictionary<Item, List<ItemSlot>> itemTypeToSlotListMap = new Dictionary<Item, List<ItemSlot>>();

    public bool IsUIOpen { get; private set; }
    public bool IsPause { get; private set; }

    public void SetUIOpen(bool isOpen)
    {
        IsUIOpen = isOpen;
        ToggleInventory();
    }

    public void SetPause(bool isPaused)
    {
        IsPause = isPaused;
        ToggleInventory();
    }

    // Start is called before the first frame update
    void Start()
    {
        Subscribe();
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

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void ToggleInventory()
    {
        // isInventory는 UI가 열려있고, 게임이 재개중일 때만 true가 된다.
        isInventory = IsUIOpen && !IsPause;

        Debug.Log($"IsUIOpen: {IsUIOpen}, IsResume: {IsPause} == isInventory: {isInventory}");
        InventoryBase.SetActive(isInventory);

        CursorManager.Instance.SetUIOpen(isInventory);
    }

    public void AcquireItem(Item _item, int _count = 1)
    {
        foreach (var slot in mySlots)
        {
            Item curitem = slot.GetItemValue();
            if (curitem != null)
            {
                if (curitem.itemName == _item.itemName)
                {
                    slot.SetSlotCount(_count);
                    return;
                }
            }
            else if (curitem == null) // 슬롯이 비어있다
            {
                slot.GetItem(_item, _count); // 아이템 획득
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
        if (_item == null) return;
        if (itemTypeToSlotListMap.TryGetValue(_item, out var itemSlotList))
        {
            var itemSlot = itemSlotList.FindLast(s => s.GetItemValue() == _item);
            if (itemSlot != null)
            {
                itemSlot.SetSlotCount(-1);
                if (_item.itemType == Item.ItemType.ETC) myTargetObj?.GetComponent<ItemDesireEvent>()?.SetItemEvent();
                if (itemSlot.itemCount < 1) itemTypeToSlotListMap[_item].Remove(itemSlot);
            }
        }
        else Debug.Log("아이템이 없습니다.");
        SortItems();
    }

    public void SortItems()
    {
        var firstEmptySlot = FindSlot(true, false);
        int emptySlotIndex = Array.FindIndex(mySlots, slot => slot.GetItemValue() == null);

        for (int i = emptySlotIndex; i < mySlots.Length; i++)
        {
            if (mySlots[i].GetItemValue() != null)
            {
                mySlots[i].SwitchSlot(firstEmptySlot);
                var tempslot = firstEmptySlot;
                firstEmptySlot = mySlots[i];
                Debug.Log(tempslot + "'s item: " + tempslot.GetItemValue());
                itemTypeToSlotListMap[tempslot.GetItemValue()].Add(tempslot);
                itemTypeToSlotListMap[tempslot.GetItemValue()].Remove(firstEmptySlot);
            }
        }
    }

    public void SetItemTargetObj(Transform target)
    {
        myTargetObj = target;
    }

    public bool IsItemExist(Item item)
    {
        if (itemTypeToSlotListMap.TryGetValue(item, out var itemSlotList))
        {
            Debug.Log(item + ", " + itemSlotList.Count);
            if (itemSlotList.Count == 0) return false;
        }
        return itemTypeToSlotListMap.ContainsKey(item);
    }

    public void Subscribe()
    {
        this.EventStartingListening<GameStatesEvent>();
        this.EventStartingListening<CameraStatesEvent>();
    }

    public void Unsubscribe()
    {
        this.EventStopListening<GameStatesEvent>();
        this.EventStopListening<CameraStatesEvent>();
    }

    public void OnEvent(CameraStatesEvent eventType)
    {
        switch (eventType.cameraEventType)
        {
            case CameraEventType.UI:
                SetUIOpen(true);
                break;
            case CameraEventType.NotUI:
                SetUIOpen(false);
                break;
        }
    }

    public void OnEvent(GameStatesEvent eventType)
    {
        switch (eventType.gameEventType)
        {
            case GameEventType.Pause:
                SetPause(true);
                break;
            case GameEventType.UnPause:
                SetPause(false);
                break;
        }
    }
}
