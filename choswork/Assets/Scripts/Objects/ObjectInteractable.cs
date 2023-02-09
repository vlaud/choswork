using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractable : InputManager
{
    [SerializeField] protected TMPro.TMP_Text actionText;  // �ൿ�� ���� �� �ؽ�Ʈ
    public string ShowMessage = " ���� ";

    protected void SetActionText()
    {
        if (actionText == null) actionText = GameManagement.Inst.myActionText;
    }
    public virtual void SetItemInfoAppear(bool v) //������Ʈ�� ������ ���� UI ���̱�
    {
        actionText?.gameObject.SetActive(v);
    }
    public virtual void SetText()
    {
        if (actionText == null) return;
        actionText.text = transform.GetComponent<ItemPickUp>()?.item.itemName + ShowMessage + "<color=yellow>" + "(E)" + "</color>";
    }
}
