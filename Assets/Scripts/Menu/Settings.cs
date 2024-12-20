using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class Settings : MonoBehaviour
{
    public static Settings Instance { get; private set; }
    public GameObject settingsMenu;
    public GameObject fpsSliderGO;
    public Slider fpsSlider;
    public TextMeshProUGUI fpsNumber;
    public GameObject fpsDisplay; // To show player current fps in game
    public GameObject vSyncSliderGO;
    public Slider vSyncSlider;
    public TextMeshProUGUI vSyncNumber;
    public Toggle displayFpsToggle;
    public Toggle screenModeToggle;
    public Toggle eggsModeToggle;
    public Button applyChangesBtn;


    public string soundPath;
    private string audioPath = "GUI";
    [SerializeField] private AudioMixer audioMixer;

    SaveSetting.DataToSave settings;
    SaveSetting.DataToSave fileSettings;
    private GameObject settingsMenuPrefab;
    private GameObject fpsDisplayOnDashboard;
    private Button exitSettings;

    public GameObject fpsGuiInstance;
    public GameObject settingMenuInstance;

    // public int vSyncOptions = 0;
    // public int frameRate = 0;
    int changeVSync;
    int frameRate;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        settingsMenuPrefab = Resources.Load<GameObject>("PreFabs/GUI/SettingMenu");
        fpsDisplayOnDashboard = Resources.Load<GameObject>("PreFabs/GUI/FpsGui");

        Setup();

    }

    void Start()
    {
        // ChangeFrameRate();
        // ChangeVsync();

        // fileSettings = SaveSetting.LoadUserSettings();
        // ApplySettings(false);
        // JsonHandler.Save();
        // JsonHandler.DataToSave data = JsonHandler.ReadFile("settings");
        // frameRate = data.frameRate;
        // vsyncOptions = data.vsyncOption;
    }

    void Update()
    {
        if (settingsMenu == null)
            Setup();
    }

    public void Setup()
    {
        if (settingsMenu == default)
        {
            settingMenuInstance = Instantiate(settingsMenuPrefab, Vector3.zero, Quaternion.identity);
            settingMenuInstance.name = "SettingMenu";
            fpsGuiInstance = Instantiate(fpsDisplayOnDashboard, Vector3.zero, Quaternion.identity);
            fpsGuiInstance.name = "FpsGui";
        }
        settingsMenu = settingMenuInstance;
        fpsDisplay = fpsGuiInstance;

        // Initalizes fps sldier related things at the beginning
        fpsSliderGO = GameObject.Find("FpsSlider");
        fpsSlider = fpsSliderGO.GetComponent<Slider>();
        fpsSlider.onValueChanged.AddListener(ChangeFrameRate);
        fpsNumber = fpsSliderGO.transform.Find("Fps").gameObject.GetComponent<TextMeshProUGUI>();

        // Initalizes vsync sldier related things at the beginning
        vSyncSliderGO = GameObject.Find("VSyncSlider");
        vSyncSlider = vSyncSliderGO.GetComponent<Slider>();
        vSyncSlider.onValueChanged.AddListener(ChangeVSync);
        vSyncNumber = vSyncSliderGO.transform.Find("VSync").gameObject.GetComponent<TextMeshProUGUI>();

        displayFpsToggle = GameObject.Find("DisplayFpsToggle").GetComponent<Toggle>();
        displayFpsToggle.onValueChanged.AddListener(DisplayFps);
        screenModeToggle = GameObject.Find("FullScreenToggle").GetComponent<Toggle>();
        screenModeToggle.onValueChanged.AddListener(ToggleScreen);
        eggsModeToggle = GameObject.Find("EggsToggle").GetComponent<Toggle>();
        eggsModeToggle.onValueChanged.AddListener(ChangeEggsMode);

        applyChangesBtn = GameObject.Find("SaveSettingsBtn").GetComponent<Button>();
        applyChangesBtn.onClick.AddListener(ApplySettingsAndSave);
        exitSettings = GameObject.Find("CloseSettingButton").GetComponent<Button>();
        exitSettings.onClick.AddListener(CloseSetting);

        // Gets everything from file
        settings = SaveSetting.LoadUserSettings();
        // Initalizes it
        ApplySettings();
        settingsMenu.SetActive(false);
    }

    private void CloseSetting()
    {
        if (GameMenu.Instance != null)
        {
            GameMenu.Instance.CloseSetting();
            return;
        }
        //Fall Back
        SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 1f);
        settingsMenu.SetActive(false);
    }

    private void ChangeFrameRate(float a)
    {
        settings.frameRate = (int)fpsSlider.value;
        FrameRateApply();
    }

    public void FrameRateApply(bool apply = false)
    {
        if (settings.vSyncLevel != 0 && settings.frameRate != 0)
        {
            vSyncNumber.text = "Off";
            if (apply)
                QualitySettings.vSyncCount = 0;
            vSyncSlider.value = 0;
            settings.vSyncLevel = 0;
        }

        if (fpsSlider.value == 9)
        {
            fpsNumber.text = "Unlimited";
            if (apply)
                Application.targetFrameRate = 0;
            settings.frameRate = 0;
        }
        else
        {
            fpsNumber.text = settings.frameRate.ToString();
            if (apply)
                Application.targetFrameRate = settings.frameRate;
        }
    }

    public void ChangeVSync(float a)
    {
        settings.vSyncLevel = (int)vSyncSlider.value;
        VSyncApply();
    }

    public void VSyncApply(bool apply = false)
    {
        if (settings.frameRate != 0 && settings.vSyncLevel != 0)
        {
            fpsNumber.text = "Unlimited";
            if (apply)
                Application.targetFrameRate = 0;
            fpsSlider.value = 0;
            settings.frameRate = 0;
        }

        switch (settings.vSyncLevel)
        {
            case 0:
                if (apply)
                    QualitySettings.vSyncCount = 0;
                vSyncNumber.text = "Off";
                settings.vSyncLevel = 0;
                break;
            case 1:
                if (apply)
                    QualitySettings.vSyncCount = 1;
                vSyncNumber.text = "Refresh Rate";
                settings.vSyncLevel = 1;
                break;
            case 2:
                if (apply)
                    QualitySettings.vSyncCount = 2;
                vSyncNumber.text = "1/2 Refresh Rate";
                settings.vSyncLevel = 2;
                break;
            case 3:
                if (apply)
                    QualitySettings.vSyncCount = 3;
                vSyncNumber.text = "1/3 Refresh Rate";
                settings.vSyncLevel = 3;
                break;
            case 4:
                if (apply)
                    QualitySettings.vSyncCount = 4;
                vSyncNumber.text = "1/4 Refresh Rate";
                settings.vSyncLevel = 4;
                break;
        }
    }

    public void DisplayFps(bool isOn)
    {
        if (SoundFXManager.instance != null)
            SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 1f);
        settings.displayFps = isOn;
    }

    public void ToggleScreen(bool isOn)
    {
        if (SoundFXManager.instance != null)
            SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 1f);
        settings.fullScreen = isOn;
        // Screen.fullScreen = isOn;

        // if (!Screen.fullScreen)
        // {
        //     Screen.SetResolution(640, 480, false);
        // }
        // else
        // {
        //     Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
        // }
    }
    public void ChangeScreenResoultion()
    {
        if (SoundFXManager.instance != null)
            SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 1f);
        Screen.SetResolution(640, 480, false);
    }

    public void ChangeEggsMode(bool isOn)
    {
        if (SoundFXManager.instance != null)
            SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 1f);
        settings.eggsMode = isOn;
    }

    public void ApplySettings()
    {
        fpsDisplay.SetActive(settings.displayFps);
        Screen.fullScreen = settings.fullScreen;

        if (settings.eggsMode)
        {
            soundPath = "EggsSoundFX";
        }
        else
        {
            soundPath = "SoundFX";
        }

        vSyncSlider.value = settings.vSyncLevel;
        fpsSlider.value = settings.frameRate;

        VSyncApply(true);
        FrameRateApply(true);

        // Updates all toggles to reflect the proper values
        displayFpsToggle.isOn = settings.displayFps;
        screenModeToggle.isOn = settings.fullScreen;
        eggsModeToggle.isOn = settings.eggsMode;

        // TODO: have player confirm settings before saving them incase it is bad
    }

    public void ApplySettingsAndSave()
    {
        ApplySettings();
        SaveSetting.Save(settings.displayFps, settings.fullScreen, settings.eggsMode, settings.frameRate, settings.vSyncLevel);
    }

    public void SetMasterVolume(float level)
    {

    }
    public void SetSoundFXVolume(float level)
    {

    }
    public void SetMusicVolume(float level)
    {

    }
}
