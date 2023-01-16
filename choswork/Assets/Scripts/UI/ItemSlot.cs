using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Item item; // »πµÊ«— æ∆¿Ã≈€
    public int itemCount; // »πµÊ«— æ∆¿Ã≈€¿« ∞≥ºˆ
    [SerializeField] private Text text_Count;
    [SerializeField] private GameObject go_CountImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetChildren(Transform child)
    {
        child.SetParent(transform);
        child.localPosition = Vector3.zero;
    }
}
