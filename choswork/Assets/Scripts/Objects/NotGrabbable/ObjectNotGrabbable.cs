using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectNotGrabbable : ObjectInteractable
{
    public virtual void Interact()
    {

    }
    protected void SetActionText()
    {
        if (actionText == null) actionText = GameManagement.Inst.myActionText;
    }
}
