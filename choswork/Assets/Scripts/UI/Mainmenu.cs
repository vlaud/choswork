using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Mainmenu : Singleton<Mainmenu>
{
    Animator anim;
    Animator faderAnim;

    public string newGameSceneName;
    public int quickSaveSlotID;

    [Header("옵션 설정")]
    public GameObject MainScreenPanel;
    public GameObject MainOptionsPanel;
    public GameObject StartGameOptionsPanel;
    public GameObject GamePanel;
    public GameObject ControlsPanel;
    public GameObject GfxPanel;
    public GameObject LoadGamePanel;
    public GameObject Fader;
    [Header("BGM 설정")]
    public AudioClip titleBGM;
    public AudioClip InGameBGM;
    public AudioClip DangerBGM;
    [Header("슬라이더 목록")]
    public Slider[] GamePanel_Sliders;

    public enum State
    {
        Create, Menu, Options, SubOptions
    }
    public State myState = State.Create;

    public void ChangeState(State s)
    {
        if (myState == s) return;
        myState = s;

        switch (myState)
        {
            case State.Menu:
                break;
            case State.Options:
                break;
            case State.SubOptions:
                break;
        }
    }
    private void Awake()
    {
        base.Initialize();
    }
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        faderAnim = Fader.GetComponent<Animator>();
        //new key
        PlayerPrefs.SetInt("quickSaveSlot", quickSaveSlotID);
        GamePanel_Sliders = GamePanel.transform.GetComponentsInChildren<Slider>();
        GamePanel_Sliders[0].value = GraphicManager.Inst.fbrightness;
        GamePanel_Sliders[1].value = GraphicManager.Inst.fconstrast;
        GamePanel_Sliders[2].value = SoundManager.Inst.bgmVolume;
        GamePanel_Sliders[3].value = SoundManager.Inst.effectVolume;
        ChangeState(State.Menu);
    }
    #region Scene Change
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        print("Scene has loaded");
    }
    private void OnDestroy()
    {
        anim = null;
        faderAnim = null;
    }
    public void PlayDangerMusic()
    {
        SoundManager.Inst.PlayBGM(DangerBGM);
    }
    public void PlayInGameMusic()
    {
        SoundManager.Inst.PlayBGM(InGameBGM);
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
            newGameSceneName = "GameStage";
            faderAnim?.SetTrigger("FadeIn");
            ShowMenuAnim(true);
            Cursor.lockState = CursorLockMode.None;
            SoundManager.Inst.PlayBGM(titleBGM);
        }
        if (scene.name == "GameStage")
        {
            ChangeState(State.Menu);
            SoundManager.Inst.PlayBGM(InGameBGM);
            GamePanel_Sliders[0].onValueChanged.AddListener((float v) => GraphicManager.Inst.fbrightness = v);
            GamePanel_Sliders[1].onValueChanged.AddListener((float v) => GraphicManager.Inst.fconstrast = v);
            GamePanel_Sliders[2].onValueChanged.AddListener((float v) => SoundManager.Inst.bgmVolume = v);
            GamePanel_Sliders[3].onValueChanged.AddListener((float v) => SoundManager.Inst.effectVolume = v);
            newGameSceneName = "testScene";
            faderAnim?.SetTrigger("FadeIn");
            DisableUI();
        }
        if (scene.name == "testScene")
        {
            faderAnim.SetTrigger("FadeIn");
        }
    }
    public void ShowMenuAnim(bool v)
    {
        if(v) anim?.Play("buttonTweenAnims_off");
        else anim?.Play("buttonTweenAnims_on");
    }
    public void DisableUI()
    {
        MainScreenPanel.SetActive(false);
        MainOptionsPanel.SetActive(false);
        StartGameOptionsPanel.SetActive(false);
        GamePanel.SetActive(false);
        ControlsPanel.SetActive(false);
        GfxPanel.SetActive(false); ;
        LoadGamePanel.SetActive(false);
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
        ChangeState(State.Options);
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
        ChangeState(State.Options);
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
        ChangeState(State.SubOptions);
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
        ChangeState(State.SubOptions);
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
        ChangeState(State.SubOptions);
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
        ChangeState(State.SubOptions);
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
        ChangeState(State.Menu);
    }

    public void back_options_panels()
    {
        //simply play anim for CLOSING main options panel
        anim.Play("OptTweenAnim_off");

        //play click sfx
        playClickSound();
        ChangeState(State.Options);
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
