using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : MonoBehaviour
{
    static public Toggle Inst = null;
    private void Awake()
    {
        Inst = this;
    }
    public void Toggling(ref bool b) // ��� ���
    {
        b = !b;
    }
}
