using UnityEngine;

public class CutScenePlayer : MonoBehaviour
{
    public Animator anim;
    public Animator CarAnim;
    public Animator CarDoorAnim;
    public void CarOut()
    {
        anim.SetTrigger("CarOut");
        transform.SetParent(null);
    }
    public void CarOpen()
    {
        CarDoorAnim.SetTrigger("Open");
    }
    public void CarClose()
    {
        CarDoorAnim.SetTrigger("Close");
    }
    public void FadeToLevel()
    {
        GameManagement.Inst.myMainmenu?.FadeToLevel();
    }
}
