using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractable : InputManager
{
    [SerializeField] protected TMPro.TMP_Text actionText;  // 행동을 보여 줄 텍스트
    public string ShowMessage = " 열기 ";

    protected void SetActionText()
    {
        if (actionText == null) actionText = GameManagement.Inst.myActionText;
    }
    public virtual void SetItemInfoAppear(bool v) //오브젝트와 아이템 집기 UI 보이기
    {
        actionText?.gameObject.SetActive(v);
    }
    public virtual void SetText()
    {
        if (actionText == null) return;
        actionText.text = transform.GetComponent<ItemPickUp>()?.item.itemName + ShowMessage + "<color=yellow>" + "(E)" + "</color>";
    }
}
