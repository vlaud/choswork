using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : ObjectNotGrabbable
{
    public Dialogue _dialogue;
    [SerializeField] private bool isTalking = false;
    [SerializeField] private int curDialogueTracker = 0;
    public GameObject diaglogueUI;

    public TMPro.TMP_Text journalText;
    // Start is called before the first frame update
    void Start()
    {
        SetActionText();
        DisableUI();
    }
    public override void Interact()
    {
        Cursor.lockState = CursorLockMode.None;
        if (!isTalking) StartJournal();
        else DisableUI();
    }
    public override void DisableUI()
    {
        diaglogueUI.SetActive(false);
        isTalking = false;
        DesireCursorState(GameManagement.GameState.Play, CursorLockMode.Locked);
    }
    void StartJournal()
    {
        isTalking = true;
        curDialogueTracker = 0;
        diaglogueUI.SetActive(true);
        journalText.text = "<color=#000000>" + _dialogue.dialogue[0] + "</color>";
    }
    public override void LeftMouseClickEvent()
    {
        curDialogueTracker--;
        if (curDialogueTracker < 0)
        {
            curDialogueTracker = 0;
        }
        journalText.text = "<color=#000000>" + _dialogue.dialogue[curDialogueTracker] + "</color>";
    }
    public override void RightMouseClickEvent()
    {
        curDialogueTracker++;
        if (curDialogueTracker > _dialogue.dialogue.Length - 1)
        {
            curDialogueTracker = _dialogue.dialogue.Length - 1;
        }
        journalText.text = "<color=#000000>" + _dialogue.dialogue[curDialogueTracker] + "</color>";
    }
}
