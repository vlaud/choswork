using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keypad : ObjectNotGrabbable
{
    [SerializeField] protected TMPro.TMP_Text passwordText;
    // Start is called before the first frame update
    void Start()
    {
        SetActionText();
        if (passwordText == null) passwordText = GameObject.Find("passwordtext").GetComponent<TMPro.TMP_Text>();
        List<int> passNums = GetRandomNumber.GetRanNum(1, 10, 4, false);
        string Password = "";
        foreach (int num in passNums)
        {
            Password += num.ToString();
        }
        
        passwordText.text = Password;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
