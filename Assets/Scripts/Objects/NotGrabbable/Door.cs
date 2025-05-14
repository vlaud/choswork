public class Door : ObjectNotGrabbable, ItemDesireEvent
{
    public Item requiredItem;
    public int requiredCount;
    // Start is called before the first frame update
    void Start()
    {
        SetActionText();
    }
    public override void Interact()
    {
        Debug.Log("door key used");
        GameManagement.Inst.myInventory.DestroyItem(requiredItem);
        GameManagement.Inst.GameClear();
    }
    public void SetItemEvent()
    {
        GameManagement.Inst.IsGameClear = true;
    }
    public override void SetText()
    {
        if (!GameManagement.Inst.myInventory.IsItemExist(requiredItem))
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
        GameManagement.Inst.myInventory.GetComponent<ItemTargeting>()?.SetItemTargetObj(transform);
    }
}
