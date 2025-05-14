using UnityEngine;

[CreateAssetMenu(fileName = "New Object", menuName = "Object/New Object")]
public class ObjectScriptable : ScriptableObject
{
    public enum ObjectTypes
    {
        Item, Image, Interactable
    }
    public ObjectTypes type;
    public string ObjectName;
}
