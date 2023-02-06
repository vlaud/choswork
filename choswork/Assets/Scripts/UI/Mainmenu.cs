using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Mainmenu : MonoBehaviour
{
    Animator anim;
    Animator faderAnim;

    public string newGameSceneName;
    public int quickSaveSlotID;

    [Header("옵션 설정")]
    public GameObject MainOptionsPanel;
    public GameObject StartGameOptionsPanel;
    public GameObject GamePanel;
    public GameObject ControlsPanel;
    public GameObject GfxPanel;
    public GameObject LoadGamePanel;
    public GameObject Fader;
    [Header("BGM 설정")]
    public AudioClip titleBGM;
    [Header("슬라이더 목록")]
    public Slider[] Sliders;
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        faderAnim = Fader.GetComponent<Animator>();
        //new key
        PlayerPrefs.SetInt("quickSaveSlot", quickSaveSlotID);
        Sliders = GamePanel.transform.GetComponentsInChildren<Slider>();
    }
    #region Scene Change
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        print("Scene has loaded");
    }
    public void FadeToLevel()
    {
        faderAnim.SetTrigger("FadeOut");
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print("Scene on scene");
        if (scene.name == "Title")
        {
            SoundManager.Inst.PlayBGM(titleBGM);
        }
        if (scene.name == "GameStage")
        {
            newGameSceneName = "testScene";
            faderAnim.SetTrigger("FadeIn");
            DisableUI();
        }
        if (scene.name == "testScene")
        {
            faderAnim.SetTrigger("FadeIn");
        }
    }
    void DisableUI()
    {
        MainOptionsPanel.SetActive(false);
        StartGameOptionsPanel.SetActive(false);
        GamePanel.SetActive(false);
        ControlsPanel.SetActive(false);
        GfxPanel.SetActive(false); ;
        LoadGamePanel.SetActive(false);
        //Fader.SetActive(false);
    }
    #endregion

    #region Open Different panels

    public void openOptions()
    {
        //enable respective panel
        MainOptionsPanel.SetActive(true);
        StartGameOptionsPanel.SetActive(false);

        //play anim for opening main options panel
        anim.Play("buttonTweenAnims_on");

        //play click sfx
        playClickSound();

        //enable BLUR
        //Camera.main.GetComponent<Animator>().Play("BlurOn");

    }

    public void openStartGameOptions()
    {
        //enable respective panel
        MainOptionsPanel.SetActive(false);
        StartGameOptionsPanel.SetActive(true);

        //play anim for opening main options panel
        anim.Play("buttonTweenAnims_on");

        //play click sfx
        playClickSound();

        //enable BLUR
        //Camera.main.GetComponent<Animator>().Play("BlurOn");

    }

    public void openOptions_Game()
    {
        //enable respective panel
        GamePanel.SetActive(true);
        ControlsPanel.SetActive(false);
        GfxPanel.SetActive(false);
        LoadGamePanel.SetActive(false);

        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();

    }
    public void openOptions_Controls()
    {
        //enable respective panel
        GamePanel.SetActive(false);
        ControlsPanel.SetActive(true);
        GfxPanel.SetActive(false);
        LoadGamePanel.SetActive(false);

        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();

    }
    public void openOptions_Gfx()
    {
        //enable respective panel
        GamePanel.SetActive(false);
        ControlsPanel.SetActive(false);
        GfxPanel.SetActive(true);
        LoadGamePanel.SetActive(false);

        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();

    }

    public void openContinue_Load()
    {
        //enable respective panel
        GamePanel.SetActive(false);
        ControlsPanel.SetActive(false);
        GfxPanel.SetActive(false);
        LoadGamePanel.SetActive(true);

        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();

    }

    public void newGame()
    {
        if (!string.IsNullOrEmpty(newGameSceneName))
        {
            GameObject obj = GameObject.Find("SceneLoader");
            transform.parent.SetParent(obj.transform);
            SceneLoader.Inst.ChangeScene(newGameSceneName);
        }
        else
            Debug.Log("Please write a scene name in the 'newGameSceneName' field of the Main Menu Script and don't forget to " +
                "add that scene in the Build Settings!");
    }
    #endregion

    #region Back Buttons

    public void back_options()
    {
        //simply play anim for CLOSING main options panel
        anim.Play("buttonTweenAnims_off");

        //disable BLUR
        // Camera.main.GetComponent<Animator>().Play("BlurOff");

        //play click sfx
        playClickSound();
    }

    public void back_options_panels()
    {
        //simply play anim for CLOSING main options panel
        anim.Play("OptTweenAnim_off");

        //play click sfx
        playClickSound();

    }

    public void Quit()
    {
        Application.Quit();
    }
    #endregion

    #region Sounds
    public void playHoverClip()
    {

    }

    void playClickSound()
    {

    }


    #endregion
}
