using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    public bool IsUIOpen { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsPopupOpen { get; private set; }

    private void Awake()
    {
        Instance = this;
        UpdateCursorState();
    }

    public void SetUIOpen(bool isOpen)
    {
        IsUIOpen = isOpen;
        UpdateCursorState();
    }

    public void SetPaused(bool isPaused)
    {
        IsPaused = isPaused;
        UpdateCursorState();
    }

    public void SetPopupOpen(bool isOpen)
    {
        IsPopupOpen = isOpen;
        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        if (IsUIOpen || IsPaused || IsPopupOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public bool IsCurSorLocked()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }
}
