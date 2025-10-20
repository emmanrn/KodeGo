using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game_Configuration
{
    public static Game_Configuration activeConfig;

    public static string FILE_PATH => $"{FilePaths.root}gameconf.cfg";

    public const bool ENCRYPT = false;

    // General Settings
    public bool displayFullscreen = true;
    public string displayResolution = "1920x1080";
    public bool continueSkippingAfterChoice = false;
    public float dialogueTextSpeed = 1f;
    public float dialogueAutoReaderSpeed = 1f;


    // Audio Settings (again still not implemented but just in case)
    public float musicVolume = 1f;
    public bool musicMute = false;

    // Other Settings
    public float historyLogScale = 1f;

    public void Load()
    {
        var ui = ConfigMenu.instance.ui;

        // GENERAL
        // Set Window size
        ConfigMenu.instance.SetDisplayToFullscreen(displayFullscreen);
        // only set fullscreen only if displayFullScreen is true
        ui.SetButtonColors(ui.fullscreen, ui.windowed, displayFullscreen);

        // Set screen resolution
        int resIndex = 0;
        for (int i = 0; i < ui.resolutions.options.Count; i++)
        {
            string resolution = ui.resolutions.options[i].text;
            if (resolution == displayResolution)
            {
                resIndex = i;
                break;
            }
        }

        ui.resolutions.value = resIndex;

        // set continue after skipping option
        ui.SetButtonColors(ui.skippingContinue, ui.skippingStop, continueSkippingAfterChoice);

        // set the value for the dialogue text speed and auto reader
        ui.architectSpeed.value = dialogueTextSpeed;
        ui.autoReaderSpeed.value = dialogueAutoReaderSpeed;

        // set music volume if ever implemented
        // ui.musicVolume.value = musicVolume;
    }

    public void Save()
    {
        FileManager.Save(FILE_PATH, JsonUtility.ToJson(this));
    }
}