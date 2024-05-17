using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    //싱글톤 패턴 - 프로그램상에 단 하나의 유일한 인스턴스를 만든다.
    static T _inst = null;
    public static T Inst
    {
        get
        {
            if (_inst == null)
            {
                Debug.Log("_inst == null");
                _inst = FindObjectOfType<T>();
                if (_inst == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).ToString();
                    _inst = obj.AddComponent<T>();
                }
            }
            return _inst;
        }
    }

    protected void Initialize()
    {
        if (_inst == null)
        {
            _inst = this as T;
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
        }
    }

}
