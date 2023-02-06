using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectNotGrabbable : ObjectInteractable
{
    public bool IsGhost = false;
    public virtual void Interact(){}
    public virtual void InteractwithMob() { }
    protected void SetActionText()
    {
        if (actionText == null) actionText = GameManagement.Inst.myActionText;
    }
    public void SetGhost(bool v) {IsGhost = v;}
    public virtual void GhostBehaviour(ObjectNotGrabbable original){}
    public virtual void GhostBehaviour() { }
    private void Update()
    {
        HandleUI();
    }
}
