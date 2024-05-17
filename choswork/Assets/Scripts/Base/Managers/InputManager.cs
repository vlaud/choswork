using System.Collections.Generic;
using UnityEngine;
using Commands.Menu;
using Commands.Camera;

public class InputManager : MonoBehaviour
{
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
    /// �÷��̾� ������
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
    /// ������Ʈ ��ȣ�ۿ�
    /// </summary>
    private void HandleObjectPickupAndThrow()
    {
        if (GameManagement.Inst.myGameState == GameState.Pause) return;

        // �÷��̾� ������Ʈ ���� + ������
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
    /// ī�޶�
    /// </summary>
    private void HandleCameraSwitching()
    {
        if (GameManagement.Inst.myGameState == GameState.Pause) return;

        // ī�޶�
        if (Input.GetKeyDown(KeyCode.V))
        {
            CameraActions.ExecuteVKey();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            CameraActions.ExecuteIKey();
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
            UIStatesEvent.Trigger(UIEventType.Disable);
        }

        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape))
        {
            UIStatesEvent.Trigger(UIEventType.Disable);
        }

        if (Input.GetMouseButtonDown(1))
        {
            UIStatesEvent.Trigger(UIEventType.RightClick);
        }
        if (Input.GetMouseButtonDown(0))
        {
            UIStatesEvent.Trigger(UIEventType.LeftClick);
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
}