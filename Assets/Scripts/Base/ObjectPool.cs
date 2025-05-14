using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    Dictionary<string, Queue<GameObject>> myPool = new Dictionary<string, Queue<GameObject>>();
    public T GetObject<T>(GameObject org, Transform parent, string orgName)
    {
        Debug.Log(orgName);
        if (myPool.ContainsKey(orgName))
        {
            if (myPool[orgName].Count > 0)
            {
                GameObject obj = myPool[orgName].Dequeue();
                obj.SetActive(true);
                obj.transform.SetParent(parent);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                return obj.GetComponent<T>();
            }
        }
        else
        {
            myPool[orgName] = new Queue<GameObject>();
        }

        return Instantiate(org, parent).GetComponent<T>();
    }
    public T GetObject<T>(GameObject org, string orgName, Vector3 pos, Quaternion rot)
    {
        if (myPool.ContainsKey(orgName))
        {
            if (myPool[orgName].Count > 0)
            {
                GameObject obj = myPool[orgName].Dequeue();
                obj.SetActive(true);
                obj.transform.SetParent(null);
                obj.transform.position = pos;
                obj.transform.rotation = rot;
                return obj.GetComponent<T>();
            }
        }
        else
        {
            myPool[orgName] = new Queue<GameObject>();
        }

        return Instantiate(org, pos, rot).GetComponent<T>();
    }
    public T GetObject<T>(GameObject org, Vector3 pos, Quaternion rot)
    {
        string Name = typeof(T).ToString();
        if(myPool.ContainsKey(Name))
        {
            if (myPool[Name].Count > 0)
            {
                GameObject obj = myPool[Name].Dequeue();
                obj.SetActive(true);
                obj.transform.SetParent(null);
                obj.transform.position = pos;
                obj.transform.rotation = rot;
                return obj.GetComponent<T>();
            }
        }
        else
        {
            myPool[Name] = new Queue<GameObject>();
        }

        return Instantiate(org, pos, rot).GetComponent<T>();
    }
    public void ReleaseObject<T>(GameObject obj, string name)
    {
        Debug.Log(name);
        if (myPool.ContainsKey(name))
        {
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            myPool[name].Enqueue(obj);
        }
        else Destroy(obj);
    }
    public void ReleaseObject<T>(GameObject obj)
    {
        obj.transform.SetParent(transform);
        obj.SetActive(false);
        myPool[typeof(T).ToString()].Enqueue(obj);
    }
}
