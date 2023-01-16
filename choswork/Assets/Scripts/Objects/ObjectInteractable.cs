using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractable : InputManager
{
    [SerializeField] private TMPro.TMP_Text actionText;  // �ൿ�� ���� �� �ؽ�Ʈ
    public string ShowMessage = " ���� ";
   
    public void SetItemInfoAppear(bool v) //������Ʈ�� ������ ���� UI ���̱�
    {
        actionText.gameObject.SetActive(v);
    }
    public void SetText()
    {
        if (actionText == null) return;
        actionText.text = transform.GetComponent<ItemPickUp>()?.item.itemName + ShowMessage + "<color=yellow>" + "(E)" + "</color>";
    }
}
