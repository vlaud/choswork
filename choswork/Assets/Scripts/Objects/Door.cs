using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : ObjectInteractable, ItemEvent
{
    public Item requiredItem;
    // Start is called before the first frame update
    void Start()
    {
        myInventory = GameManagement.Inst.myInventory.gameObject;
    }
    public override void SetText()
    {
        if (actionText == null) return;
        actionText.text = transform.GetComponent<ItemPickUp>()?.item.itemName + ShowMessage + "<color=yellow>" + "(E)" + "</color>";
    }
    public void InteractDoor() 
    {
        Inventory inv = myInventory.GetComponent<Inventory>();
        inv.GetComponent<ItemEvent>()?.SetItemTargetObj(transform);
        inv.DestroyItem(requiredItem);
        GameManagement.Inst.GameClear();
    }
    public void SetItemEvent()
    {
        GameManagement.Inst.IsGameClear = true;
    }
    public void SetItemTargetObj(Transform target)
    {

    }
}
