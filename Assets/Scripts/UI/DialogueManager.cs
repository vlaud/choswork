using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : ObjectNotGrabbable, EventListener<UIStatesEvent>, iSubscription
{
    public Dialogue _dialogue;
    [SerializeField] private bool isTalking = false;
    [SerializeField] private int curDialogueTracker = 0;
    public GameObject diaglogueUI;

    public TMPro.TMP_Text journalText;
    // Start is called before the first frame update
    void Start()
    {
        Subscribe();
        SetActionText();
        DisableUI();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public override void Interact()
    {
        Cursor.lockState = CursorLockMode.None;
        if (!isTalking) StartJournal();
        else DisableUI();
    }

    public void DisableUI()
    {
        CursorManager.Instance.SetPopupOpen(false);
        diaglogueUI.SetActive(false);
        isTalking = false;
    }

    void StartJournal()
    {
        CursorManager.Instance.SetPopupOpen(true);
        isTalking = true;
        curDialogueTracker = 0;
        diaglogueUI.SetActive(true);
        journalText.text = "<color=#000000>" + _dialogue.dialogue[0] + "</color>";
    }

    public void LeftMouseClickEvent()
    {
        curDialogueTracker--;
        if (curDialogueTracker < 0)
        {
            curDialogueTracker = 0;
        }
        journalText.text = "<color=#000000>" + _dialogue.dialogue[curDialogueTracker] + "</color>";
    }

    public void RightMouseClickEvent()
    {
        curDialogueTracker++;
        if (curDialogueTracker > _dialogue.dialogue.Length - 1)
        {
            curDialogueTracker = _dialogue.dialogue.Length - 1;
        }
        journalText.text = "<color=#000000>" + _dialogue.dialogue[curDialogueTracker] + "</color>";
    }

    public void Subscribe()
    {
        this.EventStartingListening<UIStatesEvent>();
    }

    public void Unsubscribe()
    {
        this.EventStopListening<UIStatesEvent>();
    }

    public void OnEvent(UIStatesEvent eventType)
    {
        switch (eventType.uIEventType)
        {
            case UIEventType.Disable:
                DisableUI();
                break;
            case UIEventType.LeftClick:
                LeftMouseClickEvent();
                break;
            case UIEventType.RightClick:
                RightMouseClickEvent();
                break;
        }
    }
}
