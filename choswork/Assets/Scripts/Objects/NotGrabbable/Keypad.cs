using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct KeypadStat
{
    public GameObject keypadUI;
    public GameObject hintNote;
    public TMPro.TMP_Text passwordInput;
}
public class Keypad : ObjectNotGrabbable
{
    [SerializeField] protected TMPro.TMP_Text passwordText;
    public KeypadStat myKeypad;
    public Transform myHintNote;
    public Transform myKeypadUI;
    private void Awake()
    {
        SetActionText();
        myKeypadUI = Instantiate(myKeypad.keypadUI, GameManagement.Inst.myCanvas.transform).transform;
        myHintNote = Instantiate(myKeypad.hintNote).transform;
        var canvas = myHintNote.GetComponentInChildren<Canvas>();
        if (passwordText == null) passwordText = canvas.transform.Find("passwordtext").GetComponent<TMPro.TMP_Text>();
        List<int> passNums = GetRandomNumber.GetRanNum(1, 10, 4, false);
        string Password = "";
        foreach (int num in passNums)
        {
            Password += num.ToString();
        }
        passwordText.text = Password;
    }
}
