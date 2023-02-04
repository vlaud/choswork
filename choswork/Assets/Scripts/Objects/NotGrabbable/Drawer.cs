using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Drawer : ObjectNotGrabbable
{
    [SerializeField] Animator drawerAnim;
    [SerializeField] bool IsOpen;
    public bool IsDrawer;
    // Start is called before the first frame update
    void Start()
    {
        SetActionText();
        drawerAnim = transform.GetComponent<Animator>();
        drawerAnim.SetBool("IsOpen", IsOpen);
        SetOpenCloseText();
    }
    public override void Interact()
    {
        IsOpen = !IsOpen;
        SetOpenCloseText();
        drawerAnim.SetBool("IsOpen", IsOpen);
    }
    public override void InteractwithMob()
    {
        if (!IsOpen) Interact();
    }
    public void SetOpenCloseText()
    {
        if(IsOpen) ShowMessage = " ´Ý±â ";
        else ShowMessage = " ¿­±â ";
    }
    public override void GhostBehaviour(ObjectNotGrabbable original) 
    {
        if(IsGhost)
        {
            IsOpen = original.GetComponent<Animator>().GetBool("IsOpen");
            drawerAnim.SetBool("IsOpen", IsOpen);
        }
    }
}
