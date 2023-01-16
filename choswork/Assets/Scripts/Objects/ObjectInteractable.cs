using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractable : InputManager
{
    [SerializeField] private TMPro.TMP_Text actionText;  // 행동을 보여 줄 텍스트
    public string ShowMessage = " 열기 ";
   
    public void SetItemInfoAppear(bool v) //오브젝트와 아이템 집기 UI 보이기
    {
        actionText.gameObject.SetActive(v);
    }
    public void SetText()
    {
        if (actionText == null) return;
        actionText.text = transform.GetComponent<ItemPickUp>()?.item.itemName + ShowMessage + "<color=yellow>" + "(E)" + "</color>";
    }
}
