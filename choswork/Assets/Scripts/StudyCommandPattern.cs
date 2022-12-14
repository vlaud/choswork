using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    void Execute(Transform tr);
    void Undo(Transform tr);
}
public class MoveLeft : ICommand
{
    public void Execute(Transform tr) // 나중에 다 수정해야함
    {
        tr.Translate(Vector3.left);
    }
    public void Undo(Transform tr)
    {
        tr.Translate(Vector3.right);
    }
}

public class MoveRight : ICommand
{
    public void Execute(Transform tr)
    {
        tr.Translate(Vector3.right);
    }
    public void Undo(Transform tr)
    {
        tr.Translate(Vector3.left);
    }
}
public class MoveForward : ICommand
{
    public void Execute(Transform tr)
    {
        tr.Translate(Vector3.forward);
    }
    public void Undo(Transform tr)
    {
        tr.Translate(Vector3.back);
    }
}
public class MoveBack : ICommand
{
    public void Execute(Transform tr)
    {
        tr.Translate(Vector3.back);
    }
    public void Undo(Transform tr)
    {
        tr.Translate(Vector3.forward);
    }
}
public class StudyCommandPattern : MonoBehaviour
{
    static public StudyCommandPattern Inst = null;
    Stack<ICommand> Commandlist = new Stack<ICommand>();
    public Dictionary<KeyCode, ICommand> Keylist = new Dictionary<KeyCode, ICommand>(); //wasd 리스트
    public GameObject orgBullet = null;

    private void Awake()
    {
        Inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Keylist[KeyCode.A] = new MoveLeft();
        Keylist[KeyCode.D] = new MoveRight();
        Keylist[KeyCode.W] = new MoveForward();
        Keylist[KeyCode.S] = new MoveBack();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(KeyCode key in Keylist.Keys)
        {
            if(Input.GetKeyDown(key))
            {
                //Keylist[key].Execute(transform);
                //Commandlist.Push(Keylist[key]);
            }
        }
    }
}
