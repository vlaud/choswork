using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : ObjectNotGrabbable
{
    public Dialogue _dialogue;
    private bool isTalking = false;
    private float curDialogueTracker = 0f;

    public GameObject player;
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
        diaglogueUI.gameObject.SetActive(true);
    }
    public override void DisableUI()
    {
        diaglogueUI.gameObject.SetActive(false);
    }
}
