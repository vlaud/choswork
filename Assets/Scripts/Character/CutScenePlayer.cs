using UnityEngine;

public class CutScenePlayer : MonoBehaviour
{
    public Animator anim;
    public Animator CarAnim;
    public Animator CarDoorAnim;
    public CutSceneCamera myCam;
    public void CarOut()
    {
        anim.SetTrigger("CarOut");
        transform.SetParent(null);
        myCam.CarOut();
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
        GameManagement.Inst.IsGameClear = true;
        GameManagement.Inst.GameClear();
    }
}
