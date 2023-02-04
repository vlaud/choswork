using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct KeypadStat
{
    public GameObject keypadUI;
    public GameObject hintNote;
}
public class Keypad : ObjectNotGrabbable
{
    [SerializeField] protected TMPro.TMP_Text hintText;
    [SerializeField] protected TMPro.TMP_Text passwordInput;
    public KeypadStat myKeypad;
    public Transform myHintNote;
    public Transform myKeypadUI;
    public Button[] myButtons;
    private string Nr = null;
    private int NrIndex = 0;
    private void Awake()
    {
        SetActionText();
        myKeypadUI = Instantiate(myKeypad.keypadUI, GameManagement.Inst.myCanvas.transform).transform;
        if (passwordInput == null) passwordInput = myKeypadUI.Find("KeyInput").GetComponentInChildren<TMPro.TMP_Text>();
        myHintNote = Instantiate(myKeypad.hintNote).transform;
        var canvas = myHintNote.GetComponentInChildren<Canvas>();
        if (hintText == null) hintText = canvas.transform.Find("passwordtext").GetComponent<TMPro.TMP_Text>();
        List<int> passNums = GetRandomNumber.GetRanNum(1, 10, 4, false);
        string Password = "";
        foreach (int num in passNums)
        {
            Password += num.ToString();
        }
        myButtons = myKeypadUI.GetComponentsInChildren<Button>();
        ButtonClick();
        hintText.text = Password;
        DisableUI();
    }
    public override void Interact()
    {
        myKeypadUI.gameObject.SetActive(true);
    }
    public void DisableUI()
    {
        myKeypadUI.gameObject.SetActive(false);
    }
    public void CodeFunction(string Number)
    {
        NrIndex++;
        Nr = Nr + Number;
        passwordInput.text = Nr;
    }
    void ButtonClick()
    {
        for(int i = 0; i < 10; ++i)
        {
            int x = i; //원인 불명으로 i를 바로 쓰면 죄다 10으로 변함
            myButtons[x].onClick.AddListener(delegate { CodeFunction(x.ToString()); });
        }
        myButtons[10].onClick.AddListener(EnterNumber);
    }
    public void EnterNumber()
    {
        if(Nr == hintText.text)
        {
            Debug.Log("correct");
        }
    }
}
