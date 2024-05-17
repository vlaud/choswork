public class ObjectNotGrabbable : ObjectInteractable
{
    public bool IsGhost = false;
    public virtual void Interact() { }
    public virtual void InteractwithMob() { }
    public void SetGhost(bool v) { IsGhost = v; }
    public virtual void GhostBehaviour(ObjectNotGrabbable original) { }
    public virtual void GhostBehaviour() { }
}
