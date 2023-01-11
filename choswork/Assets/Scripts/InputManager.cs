using UnityEngine;

public enum CamState
{
    FPS, UI
}
public interface PickUpHandler
{
    void ThrowObject();
    void PickUpAndDrop();
    void GetItem();
}
public interface PlayerHandler
{
    void PlayerMove();
    Vector2 GetMoveRaw();
    bool IsKickKeyPressed();
    bool IsDashKeyPressed();
    Animator ReturnAnim();
}
public interface CameraHandler
{
    void ToggleCam(CamState cam);
    void DebugCamera();
    bool GetIsUI();
}
public class PlayerAction : MonoBehaviour, PickUpHandler, PlayerHandler, CameraHandler
{
    public virtual void ThrowObject()
    {

    }
    public virtual void PickUpAndDrop()
    {

    }
    public virtual void GetItem()
    {

    }
    public virtual void PlayerMove()
    {

    }
    public virtual Vector2 GetMoveRaw()
    {
        Vector2 targetDir = Vector2.zero;
        targetDir.x = Input.GetAxisRaw("Horizontal");
        targetDir.y = Input.GetAxisRaw("Vertical");
        return targetDir;
    }
    public virtual bool IsKickKeyPressed()
    {
        return Input.GetKey(KeyCode.F);
    }
    public virtual bool IsDashKeyPressed()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }
    public virtual Animator ReturnAnim()
    {
        Animator myAnim = null;
        return myAnim;
    }
    public virtual void ToggleCam(CamState cam)
    {

    }
    public virtual void DebugCamera()
    {

    }
    public virtual bool GetIsUI()
    {
        return false;
    }
}

public interface InputManagement
{
    void HandlePlayerMovement();
    void HandleObjectPickupAndThrow();
    void HandleCameraSwitching();
    void HandleUI();
    void HandleOtherInput();

}
public class InputManager : PlayerAction, InputManagement
{
    public virtual void HandlePlayerMovement()
    {
        // 플레이어 움직임
        if (IsKickKeyPressed() && !ReturnAnim().GetBool("IsKicking")) ReturnAnim().SetTrigger("Kick");
    }

    public virtual void HandleObjectPickupAndThrow()
    {
        // 플레이어 오브젝트 집기 + 던지기
        if (Input.GetMouseButtonDown(0))
        {
            ThrowObject();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUpAndDrop();
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
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            DebugCamera();
        }
    }

    public virtual void HandleUI()
    {
        // UI handling
    }

    public virtual void HandleOtherInput()
    {
        // Any other input handling
    }
}
