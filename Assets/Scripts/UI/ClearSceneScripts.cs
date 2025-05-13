using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearSceneScripts : MonoBehaviour
{
    [SerializeField] private Mainmenu mainmenu;
    [SerializeField] private Canvas canvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas = GetComponent<Canvas>();
        mainmenu = FindAnyObjectByType<Mainmenu>();

        if (mainmenu != null)
        {
            canvas.worldCamera = mainmenu.MenuCamera;
        }
    }

    public void GoTitle()
    {
        if (mainmenu != null)
        {
            mainmenu.FadeToLevel();
        }
    }
}
