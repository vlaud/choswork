using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyCommandPattern : MonoBehaviour
{
    static public StudyCommandPattern Inst = null;
    public Dictionary<KeyCode, iCommandBase> Keylist = new Dictionary<KeyCode, iCommandBase>(); //wasd ¸®½ºÆ®

    iCommandBase AKey = null;
    iCommandBase DKey = null;
    iCommandBase WKey = null;
    iCommandBase SKey = null;

    private void Awake()
    {
        Inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Keylist[KeyCode.A] = AKey;
        Keylist[KeyCode.D] = DKey;
        Keylist[KeyCode.W] = WKey;
        Keylist[KeyCode.S] = SKey;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyCode key in Keylist.Keys)
        {
            if (Input.GetKeyDown(key))
            {
                //Keylist[key].Execute(transform);
                //Commandlist.Push(Keylist[key]);
            }
        }
    }

    public static bool IsMoveKeyPressed()
    {
        var myCamera = GameManagement.Inst.mySpringArms;
        foreach (KeyCode key in Inst.Keylist.Keys)
        {
            if (myCamera.myCameraState == ViewState.UI) return false;
            if (Input.GetKey(key))
            {
                return true;
            }
        }
        return false;
    }
}
