using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct KeypadStat
{
    public GameObject keypadUI;
    public GameObject hintNote;
    public GameObject keyItem;
    public Transform keyItemPos;
}
public class Keypad : ObjectNotGrabbable, EventListener<UIStatesEvent>, iSubscription
{
    [SerializeField] protected TMPro.TMP_Text hintText;
    [SerializeField] protected TMPro.TMP_Text passwordInput;
    public KeypadStat myKeypad;
    public Transform myHintNote;
    public Transform myKeypadUI;
    public Button[] myButtons;
    private string Nr = null;
    [SerializeField] private int NrIndex = 0;

    public enum State
    {
        Create, Default, Correct
    }
    public State myState = State.Create;

    void ChangeState(State s)
    {
        if (myState == s) return;
        myState = s;

        switch (myState)
        {
            case State.Default:
                break;
            case State.Correct:
                SetUI(false);
                break;
        }
    }

    private void Awake()
    {
        SetActionText();
        myKeypadUI = Instantiate(myKeypad.keypadUI, GameManagement.Inst.myCanvas.transform).transform;
        if (passwordInput == null) passwordInput = myKeypadUI.Find("KeyInput").GetComponentInChildren<TMPro.TMP_Text>();
        myHintNote = Instantiate(myKeypad.hintNote).transform;
        var canvas = myHintNote.GetComponentInChildren<Canvas>();
        if (hintText == null) hintText = canvas.transform.Find("passwordtext").GetComponent<TMPro.TMP_Text>();
        List<int> passNums = GetRandomNumber.GetRanNums(1, 10, 4);
        string Password = "";
        foreach (int num in passNums)
        {
            Password += num.ToString();
        }
        myButtons = myKeypadUI.GetComponentsInChildren<Button>();
        ButtonClick();
        hintText.text = Password;
        SetUI(false);
        ChangeState(State.Default);
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public override void Interact()
    {
        switch (myState)
        {
            case State.Default:
                Debug.Log(hintText.text);
                SetUI(true);
                break;
            case State.Correct:
                string name = myKeypad.keyItem.GetComponent<ItemPickUp>().item.itemName;
                ObjectPool.Inst.GetObject<PickUpController>(myKeypad.keyItem, name, myKeypad.keyItemPos.position, Quaternion.identity).ReleaseObj();
                //Instantiate(myKeypad.keyItem, myKeypad.keyItemPos.position, Quaternion.identity);
                break;
        }
    }

    public void SetUI(bool v)
    {
        myKeypadUI?.gameObject.SetActive(v);
        CursorManager.Instance.SetPopupOpen(v);
    }

    public void CodeFunction(string Number)
    {
        if (NrIndex > 3) DeleteNumber();
        NrIndex++;
        Nr = Nr + Number;
        passwordInput.text = Nr;
    }
    void ButtonClick()
    {
        for (int i = 0; i < 10; ++i)
        {
            int x = i; //원인 불명으로 i를 바로 쓰면 죄다 10으로 변함
            myButtons[x].onClick.AddListener(delegate { CodeFunction(x.ToString()); });
        }
        myButtons[10].onClick.AddListener(EnterNumber);
        myButtons[11].onClick.AddListener(DeleteNumber);
    }
    public void EnterNumber()
    {
        if (Nr == hintText.text)
        {
            passwordInput.text = "correct!";
            ChangeState(State.Correct);
        }
        else
        {
            passwordInput.text = "Wrong!";
            NrIndex = 0;
            Nr = null;
        }
    }
    public void DeleteNumber()
    {
        NrIndex = 0;
        Nr = null;
        passwordInput.text = Nr;
    }
    public override void GhostBehaviour()
    {
        if (IsGhost)
        {
            Destroy(myKeypadUI.gameObject);
            myKeypadUI = null;
            Destroy(myHintNote.gameObject);
            myHintNote = null;
        }
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
                SetUI(false);
                break;
        }
    }
}
