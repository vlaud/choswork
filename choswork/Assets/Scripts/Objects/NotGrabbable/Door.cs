using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : ObjectNotGrabbable, ItemEvent
{
    public Item requiredItem;
    // Start is called before the first frame update
    void Start()
    {
        SetActionText();
        myInventory = GameManagement.Inst.myInventory.gameObject;
    }
    public override void Interact() 
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
    public void SetItemTargetObj(Transform target) { }
}
