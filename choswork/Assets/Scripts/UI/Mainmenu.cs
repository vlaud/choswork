using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Commands.Menu;

public enum MenuState
{
    Create, None, Menu, Options, SubOptions
}

public class Mainmenu : Singleton<Mainmenu>
{
    Animator anim;
    Animator faderAnim;

    private string currentSceneName;
    public string CurrentSceneName => currentSceneName;
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
    public Button SkipButton;

    [Header("슬라이더 목록")]
    public Slider[] GamePanel_Sliders;


    public MenuState myState = MenuState.Create;

    public void ChangeState(MenuState s)
    {
        if (myState == s) return;
        myState = s;

        switch (myState)
        {
            case MenuState.None:
                MenuActions.ChangeKey(MainMenuKeyType.PauseAction);
                break;
            case MenuState.Menu:
                MenuActions.ChangeKey(MainMenuKeyType.UnPauseAction);
                break;
            case MenuState.Options:
                MenuActions.ChangeKey(MainMenuKeyType.BackToMenu);
                break;
            case MenuState.SubOptions:
                MenuActions.ChangeKey(MainMenuKeyType.BackToOptions);
                break;
        }
    }

    private void SetMenuCommands()
    {
        MenuActions.SetKeys(GameManagement.Inst.myMainmenu);

        if (currentSceneName == "Title")
        {
            MenuActions.SetCommandKey(MainMenuKeyType.PauseAction, null);
            MenuActions.SetCommandKey(MainMenuKeyType.UnPauseAction, null);
            Debug.Log($"{currentSceneName} <color=red>SetMenuCommands</color>");
        }
        else Debug.Log($"{currentSceneName} <color=yellow>SetMenuCommands</color>");

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
    }

    #region Scene Change
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("Scene has loaded");
        currentSceneName = SceneManager.GetActiveScene().name;
    }
    private void OnDestroy()
    {
        anim = null;
        faderAnim = null;
    }

    public void PlayDangerMusic()
    {
        SoundManager.Inst.PlayBGM("DangerBGM");
    }

    public void PlayInGameMusic()
    {
        SoundManager.Inst.PlayBGM("InGameBGM");
    }

    public void FadeToLevel()
    {
        faderAnim.SetTrigger("FadeOut");
        MenuActions.ChangeKey(MainMenuKeyType.None);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log(currentSceneName);

        // => 여기 currentSceneName name이 바뀌고 나서
        // InputManger의 SetMenuCommands()를 실행해야 한다
        CommandManager.ClearCommands();
        SetMenuCommands();
        switch (scene.name)
        {
            case "Title":
                newGameSceneName = "CutScene";
                faderAnim?.SetTrigger("FadeIn");
                SkipButton.gameObject.SetActive(false);
                // ShowMenuAnim(bool v) => ChangeState(MenuState s) => MenuActions.ChangeKey(MenuState s)가 발동
                ShowMenuAnim(true);
                CursorManager.Instance.SetSceneTitle(true);
                SoundManager.Inst.PlayBGM("titleBGM");
                break;
            case "Loading":
                SkipButton.gameObject.SetActive(false);
                break;
            case "CutScene":
                newGameSceneName = "GameStage";
                faderAnim?.SetTrigger("FadeIn");
                SkipButton.gameObject.SetActive(true);
                DisableUI();
                break;
            case "GameStage":
                GamePanel_Sliders[0].onValueChanged.AddListener((float v) => GraphicManager.Inst.fbrightness = v);
                GamePanel_Sliders[1].onValueChanged.AddListener((float v) => GraphicManager.Inst.fconstrast = v);
                GamePanel_Sliders[2].onValueChanged.AddListener((float v) => SoundManager.Inst.bgmVolume = v);
                GamePanel_Sliders[3].onValueChanged.AddListener((float v) => SoundManager.Inst.effectVolume = v);
                newGameSceneName = "testScene";
                faderAnim?.SetTrigger("FadeIn");
                DisableUI();
                break;
            case "testScene":
                newGameSceneName = "GameStage2";
                faderAnim.SetTrigger("FadeIn");
                break;
            case "GameStage2":
                newGameSceneName = "Title";
                faderAnim.SetTrigger("FadeIn");
                break;
        }
    }
    public void ShowMenuAnim(bool v)
    {
        if (v) anim?.Play("buttonTweenAnims_off");
        else anim?.Play("buttonTweenAnims_on");

        ChangeState(v ? MenuState.Menu : MenuState.None);
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
        ChangeState(MenuState.None);
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
        ChangeState(MenuState.Options);
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
        ChangeState(MenuState.Options);
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
        ChangeState(MenuState.SubOptions);
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
        ChangeState(MenuState.SubOptions);
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
        ChangeState(MenuState.SubOptions);
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
        ChangeState(MenuState.SubOptions);
    }

    public void newGame()
    {
        if (!string.IsNullOrEmpty(newGameSceneName))
        {
            GameObject obj = GameObject.Find("SceneLoader");
            transform.parent.SetParent(obj.transform);
            SceneLoader.Inst.ChangeScene(newGameSceneName);
            Debug.Log($"{newGameSceneName} <color=red>loading</color>");
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
        ChangeState(MenuState.Menu);
    }

    public void back_options_panels()
    {
        //simply play anim for CLOSING main options panel
        anim.Play("OptTweenAnim_off");

        //play click sfx
        playClickSound();
        ChangeState(MenuState.Options);
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
