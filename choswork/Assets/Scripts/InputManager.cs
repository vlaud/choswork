using UnityEngine;

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
}
public interface CameraHandler
{
    void ToggleFPS();
    void ToggleUI();
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
    public virtual void ToggleFPS()
    {

    }
    public virtual void ToggleUI()
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
        if (Input.GetKey(KeyCode.LeftShift))
        {

        }

        if (Input.GetKeyDown(KeyCode.F))
        {

        }
        // �÷��̾� ������
    }

    public virtual void HandleObjectPickupAndThrow()
    {
        // �÷��̾� ������Ʈ ���� + ������
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
        // ī�޶�
        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleFPS();
        }
        if (Input.GetKeyDown(KeyCode.I) && GetIsUI())
        {
            ToggleUI();
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
