using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Drawer : ObjectNotGrabbable
{
    [SerializeField] Animator drawerAnim;
    [SerializeField] bool IsOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        if (actionText == null) actionText = GameManagement.Inst.myActionText;
        drawerAnim = transform.GetComponent<Animator>();
    }
    public override void Interact()
    {
        IsOpen = !IsOpen;
        SetOpenCloseText();
        drawerAnim.SetBool("IsOpen", IsOpen);
    }
    public void SetOpenCloseText()
    {
        if(IsOpen) ShowMessage = " �ݱ� ";
        else ShowMessage = " ���� ";
    }
}
