using UnityEngine;

public enum CamState
{
    FPS, UI
}
public interface ObjectHandler
{
    void ThrowObject();
    void InteractObj();
    void GetItem();
    bool IsInteractKeyPressed();
}
public interface PlayerHandler
{
    void PlayerMove();
    void TimeStop();
    Vector2 GetMoveRaw();
    bool IsKickKeyPressed();
    bool IsDashKeyPressed();
    bool IsMoveKeyPressed();
    Animator ReturnAnim();
}
public interface CameraHandler
{
    void ToggleCam(CamState cam);
    void DebugCamera();
    bool GetIsUI();
}
public interface UIHandler
{
    void ToggleInventory();
    void DisableUI();
    void LeftMouseClickEvent();
    void RightMouseClickEvent();
}
public class PlayerAction : MonoBehaviour, ObjectHandler, PlayerHandler, CameraHandler, UIHandler
{
    //ObjectHandler
    public virtual void ThrowObject() { }
    public virtual void InteractObj() { }
    public virtual void GetItem() { }
    public virtual bool IsInteractKeyPressed()
    {
        return Input.GetKeyDown(KeyCode.E);
    }
    //PlayerHandler
    public virtual void PlayerMove() { }
    public virtual void TimeStop() { }
    public virtual Vector2 GetMoveRaw()
    {
        Vector2 targetDir = Vector2.zero;
        targetDir.x = Input.GetAxisRaw("Horizontal");
        targetDir.y = Input.GetAxisRaw("Vertical");
        return targetDir;
    }
    public virtual bool IsKickKeyPressed()
    {
        return Input.GetKeyDown(KeyCode.F);
    }
    public virtual bool IsDashKeyPressed()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }
    public virtual bool IsMoveKeyPressed()
    {
        var myCamera = GameManagement.Inst.mySpringArms;
        foreach (KeyCode key in StudyCommandPattern.Inst.Keylist.Keys)
        {
            if (myCamera.myCameraState == SpringArms.ViewState.UI) return false;
            if (Input.GetKey(key))
            {
                return true;
            }
        }
        return false;
    }
    public virtual Animator ReturnAnim()
    {
        Animator myAnim = null;
        return myAnim;
    }
    //CameraHandler
    public virtual void ToggleCam(CamState cam) { }
    public virtual void DebugCamera() { }
    public virtual bool GetIsUI() { return false; }
    //UIHandler
    public virtual void ToggleInventory() { }
    public virtual void DisableUI() { }
    public virtual void LeftMouseClickEvent() { }
    public virtual void RightMouseClickEvent() { }
}

public interface InputManagement
{
    void HandlePlayerMovement();
    void HandleObjectPickupAndThrow();
    void HandleCameraSwitching();
    void HandleUI();
    void HandleOtherInput();
    void ToggleEscapeEvent();

}
public class InputManager : PlayerAction, InputManagement
{
    GameObject _inventory = null;
    protected GameObject myInventory
    {
        get => _inventory;
        set
        {
            _inventory = value;
        }
    }
    public virtual void HandlePlayerMovement()
    {
        // 플레이어 움직임
        if (Input.GetKeyDown(KeyCode.T)) TimeStop();
        if (IsKickKeyPressed() && !ReturnAnim().GetBool("IsKicking")) ReturnAnim().SetTrigger("Kick");
    }

    public virtual void HandleObjectPickupAndThrow()
    {
        // 플레이어 오브젝트 집기 + 던지기
        if (Input.GetMouseButtonDown(0))
        {
            ThrowObject();
        }
        if (IsInteractKeyPressed())
        {
            InteractObj();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            GetItem();
        }
    }
    public virtual void HandleCameraSwitching()
    {
        // 카메라
        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleCam(CamState.FPS);
        }
        if (Input.GetKeyDown(KeyCode.I) && GetIsUI())
        {
            ToggleCam(CamState.UI);
            myInventory?.GetComponent<InputManager>()?.ToggleInventory();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            DebugCamera();
        }
    }

    public virtual void HandleUI()
    {
        // UI handling
        if (IsMoveKeyPressed())
        {
            Cursor.lockState = CursorLockMode.Locked;
            DisableUI();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            DisableUI();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisableUI();
        }
        if (Input.GetMouseButtonDown(1))
        {
            RightMouseClickEvent();
        }
        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseClickEvent();
        }
    }

    public virtual void HandleOtherInput()
    {
        // Any other input handling
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            ToggleEscapeEvent();
        }
    }
    public virtual void ToggleEscapeEvent() { }
}
