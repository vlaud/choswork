using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : ObjectInteractable
{
    public Item requiredItem;
    // Start is called before the first frame update
    void Start()
    {
        myInventory = GameManagement.Inst.myInventory.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void SetText()
    {
        if (actionText == null) return;
        actionText.text = transform.GetComponent<ItemPickUp>()?.item.itemName + ShowMessage + "<color=yellow>" + "(E)" + "</color>";
    }
}
