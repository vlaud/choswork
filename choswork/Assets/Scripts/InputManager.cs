using System.Collections.Generic;
using UnityEngine;
using Commands.Menu;
using Commands.Camera;

public class InputManager : MonoBehaviour
{
    public virtual void ToggleCam(CamState cam) { }
    public virtual void DebugCamera() { }
    public virtual bool GetIsUI() { return false; }
    //UIHandler
    public virtual void ToggleInventory() { }
    public virtual void DisableUI() { }
    public virtual void LeftMouseClickEvent() { }
    public virtual void RightMouseClickEvent() { }

    Inventory _inventory = null;
    protected Inventory myInventory
    {
        get => _inventory;
        set
        {
            _inventory = value;
        }
    }

    private void Awake()
    {
        SetMenuCommands();
    }

    private void Update()
    {
        PlayerMovement();
        HandleObjectPickupAndThrow();
        HandleCameraSwitching();
        HandleUI();
        HandleOtherInput();
    }

    private void SetMenuCommands()
    {
        MenuActions.SetKeys(GameManagement.Inst.myMainmenu);

        if (GameManagement.Inst.myMainmenu?.CurrentSceneName == "Title")
        {
            MenuActions.SetCommandKey(MainMenuKeyType.PauseAction, null);
            MenuActions.SetCommandKey(MainMenuKeyType.UnPauseAction, null);
        }
    }

    /// <summary>
    /// 플레이어 움직임
    /// </summary>
    private void PlayerMovement()
    {
        if (GameManagement.Inst.myGameState == GameState.Pause) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayerStatesEvent.Trigger(PlayerEventType.TimeStop);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayerStatesEvent.Trigger(PlayerEventType.PlayerKick);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            PlayerStatesEvent.Trigger(PlayerEventType.Dash);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            PlayerStatesEvent.Trigger(PlayerEventType.NormalSpeed);
        }
    }

    /// <summary>
    /// 오브젝트 상호작용
    /// </summary>
    private void HandleObjectPickupAndThrow()
    {
        if (GameManagement.Inst.myGameState == GameState.Pause) return;

        // 플레이어 오브젝트 집기 + 던지기
        if (Input.GetMouseButtonDown(0))
        {
            ObjectStatesEvent.Trigger(ObjectEventType.ThrowObject);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ObjectStatesEvent.Trigger(ObjectEventType.InteractObj);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            ObjectStatesEvent.Trigger(ObjectEventType.GetItem);
        }
    }

    /// <summary>
    /// 수정 필요
    /// </summary>
    private void HandleCameraSwitching()
    {
        if (GameManagement.Inst.myGameState == GameState.Pause) return;

        // 카메라
        if (Input.GetKeyDown(KeyCode.V))
        {
            CameraActions.ExecuteVKey();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            CameraActions.ExecuteIKey();
            myInventory?.ToggleInventory();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CameraStatesEvent.Trigger(CameraEventType.Debug);
        }
    }

    private void HandleUI()
    {
        if (GameManagement.Inst.myMainmenu?.CurrentSceneName == "Title") return;
        if (GameManagement.Inst.myGameState == GameState.Pause) return;

        // UI handling
        if (StudyCommandPattern.IsMoveKeyPressed())
        {
            DesireCursorState(GameState.Play, CursorLockMode.Locked);
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

    private void HandleOtherInput()
    {
        // Any other input handling
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuActions.Execute();
        }
    }

    void EscapeAction2()
    {
        // Any other input handling previous
        var mainMenu = GameManagement.Inst.myMainmenu;
        if (mainMenu != null)
        {
            if (mainMenu.myState == MenuState.Menu) ToggleEscapeEvent();
            else return;
        }
        else if (mainMenu == null) ToggleEscapeEvent();
        else return;

        if (GameManagement.Inst.myGameState == GameState.Pause)
        {
            Cursor.lockState = CursorLockMode.None;
            mainMenu?.ShowMenuAnim(true);
        }
        else
        {
            if (GameManagement.Inst.myInventory.IsInventoryEnabled())
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            mainMenu?.ShowMenuAnim(false);
            mainMenu?.DisableUI();
        }
    }

    public void ToggleEscapeEvent()
    {

    }

    public void DesireCursorState(GameState state, CursorLockMode cursorState)
    {
        if (GameManagement.Inst.myGameState == state)
            Cursor.lockState = cursorState;
    }
}