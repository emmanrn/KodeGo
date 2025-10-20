using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DIALOGUE;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfigMenu : MenuPage
{
    public static ConfigMenu instance { get; private set; }
    [SerializeField] private GameObject[] panels;
    public UI_ITEMS ui;

    [SerializeField] private AnimationCurve animationCurve;
    private GameObject activePanel;
    private Game_Configuration config => Game_Configuration.activeConfig;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // making only the first panel active
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == 0);
        }

        activePanel = panels[0];

        SetAvailableResolutions();

        LoadConfig();
    }

    private void LoadConfig()
    {
        if (File.Exists(Game_Configuration.FILE_PATH))
            Game_Configuration.activeConfig = FileManager.Load<Game_Configuration>(Game_Configuration.FILE_PATH);
        else
            Game_Configuration.activeConfig = new Game_Configuration();

        Game_Configuration.activeConfig.Load();
    }

    private void OnApplicationQuit()
    {
        Game_Configuration.activeConfig.Save();
        Game_Configuration.activeConfig = null;
    }

    public void OpenPanel(string panelName)
    {
        GameObject panel = panels.First(p => p.name.ToLower() == panelName.ToLower());

        if (panel == null)
        {
            Debug.LogWarning($"Did not find panel called '{panelName}' in config menu");
            return;
        }


        if (activePanel != null && activePanel != panel)
            activePanel.SetActive(false);

        panel.SetActive(true);
        activePanel = panel;
    }

    private void SetAvailableResolutions()
    {
        Resolution[] resolutions = Screen.resolutions;
        List<string> options = new List<string>();

        for (int i = resolutions.Length - 1; i >= 0; i--)
        {
            options.Add($"{resolutions[i].width}x{resolutions[i].height}");
        }

        ui.resolutions.ClearOptions();
        ui.resolutions.AddOptions(options);

    }

    [System.Serializable]
    public class UI_ITEMS
    {
        private static Color buttonSelectedColor = new Color(1, 1, 0, 1);
        private static Color buttonUnselectedColor = new Color(1, 1, 1, 1);
        private static Color textSelectedColor = new Color(0.20f, 0.20f, 0.20f, 1);
        private static Color textUnselectedColor = new Color(0.20f, 0.20f, 0.20f, 1);
        [Header("General")]
        public Button fullscreen;
        public Button windowed;
        public TMP_Dropdown resolutions;
        public Button skippingContinue, skippingStop;
        public Slider architectSpeed, autoReaderSpeed;

        // not implemented yet, but add more once audio gets implemented
        [Header("Audio")]
        public Slider musicVolume;

        public void SetButtonColors(Button A, Button B, bool selectedA)
        {
            A.GetComponent<Image>().color = selectedA ? buttonSelectedColor : buttonUnselectedColor;
            B.GetComponent<Image>().color = !selectedA ? buttonSelectedColor : buttonUnselectedColor;

            A.GetComponentInChildren<TextMeshProUGUI>().color = selectedA ? textSelectedColor : textUnselectedColor;
            B.GetComponentInChildren<TextMeshProUGUI>().color = !selectedA ? textSelectedColor : textUnselectedColor;
        }
    }

    // UI CALLABLE FUNCTIONS
    public void SetDisplayToFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        ui.SetButtonColors(ui.fullscreen, ui.windowed, fullscreen);
    }

    public void SetDisplayResolution()
    {
        string resolution = ui.resolutions.captionText.text;
        string[] values = resolution.Split('x');

        if (int.TryParse(values[0], out int width) && int.TryParse(values[1], out int height))
        {
            Screen.SetResolution(width, height, Screen.fullScreen);
            config.displayResolution = resolution;
        }
        else
            Debug.LogError($"Parsing error for screen resolution [{resolution}] could not be parsed into WIDTHxHEIGHT");
    }

    public void SetContinueSkippingAfterChoice(bool continueSkipping)
    {
        config.continueSkippingAfterChoice = continueSkipping;
        ui.SetButtonColors(ui.skippingContinue, ui.skippingStop, continueSkipping);
    }

    public void SetTextArchitectSpeed()
    {
        config.dialogueTextSpeed = ui.architectSpeed.value;

        if (DialogueSystem.instance != null)
            DialogueSystem.instance.conversationManager.archi.speed = config.dialogueTextSpeed;
    }

    public void SetAutoReaderSpeed()
    {
        config.dialogueAutoReaderSpeed = ui.autoReaderSpeed.value;

        if (DialogueSystem.instance == null)
            return;

        AutoReader autoReader = DialogueSystem.instance.autoReader;
        if (autoReader != null)
            autoReader.speed = config.dialogueAutoReaderSpeed;
    }
}
