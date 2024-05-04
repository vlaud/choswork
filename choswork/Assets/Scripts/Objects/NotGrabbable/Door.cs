using UnityEngine;

public class Door : ObjectNotGrabbable, ItemEvent
{
    public Item requiredItem;
    public int requiredCount;
    // Start is called before the first frame update
    void Start()
    {
        SetActionText();
        myInventory = GameManagement.Inst.myInventory;
    }
    public override void Interact() 
    {
        Debug.Log("door key used");
        Inventory inv = myInventory.GetComponent<Inventory>();
        inv.DestroyItem(requiredItem);
        GameManagement.Inst.GameClear();
    }
    public void SetItemEvent()
    {
        GameManagement.Inst.IsGameClear = true;
    }
    public override void SetText()
    {
        if (!myInventory.IsItemExist(requiredItem))
        {
            ShowMessage = " ������ �ʿ� ";
            actionText.text = transform.GetComponent<ItemPickUp>()?.item.itemName + ShowMessage;
        }
        else
        {
            ShowMessage = " ���� ";
            actionText.text = transform.GetComponent<ItemPickUp>()?.item.itemName
                + ShowMessage + "<color=yellow>" + "(E)" + "</color>";
        }
    }
    public override void SetItemInfoAppear(bool v)
    {
        actionText?.gameObject.SetActive(v);
        myInventory.GetComponent<ItemTargeting>()?.SetItemTargetObj(transform);
    }
}
