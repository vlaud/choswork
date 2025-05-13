public class PickUpController : ObjectGrabbable
{
    private ItemPickUp myItem;
    private void Start()
    {
        //myInventory = GameManagement.Inst.myInventory;
        for (int i = 0; i < GameManagement.Inst.myMonsters.Length; ++i)
            hearings.Add(GameManagement.Inst.myMonsters[i].GetComponent<AIAction>().HearingSound);
        SetActionText();
    }
    public void CanPickUp()
    {
        myItem = GetComponent<ItemPickUp>();
        Debug.Log(myItem.item.itemName + " 획득 했습니다.");  // 인벤토리 넣기
        ObjectPool.Inst.ReleaseObject<PickUpController>(gameObject, myItem.item.itemName);
        //Destroy(gameObject);
        SetItemInfoAppear(false);
        GameManagement.Inst.myInventory?.GetComponent<Inventory>().AcquireItem(myItem.item);
    }
}
