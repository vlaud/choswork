using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : InputManager
{
    [SerializeField] private GameObject InventoryBase;
    [SerializeField] private bool isInventory = false;
    // Start is called before the first frame update
    void Start()
    {
        InventoryBase.SetActive(false);
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
}
