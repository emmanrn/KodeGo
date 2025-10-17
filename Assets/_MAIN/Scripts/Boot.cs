using System.Collections;
using System.Collections.Generic;
using MAIN_GAME;
using UnityEngine;
using UnityEngine.UI;

public class Boot : MonoBehaviour
{
    public GameSave save;
    [SerializeField] private Button continueButton;
    public LevelDatabase_SO levelDB;
    public bool isNewGame => GameSave.activeFile.newGame;
    void Awake()
    {
        if (GameSave.activeFile != null)
            return;

        save = GameSave.Load($"{FilePaths.gameSaves}save{GameSave.FILE_TYPE}", activateOnLoad: false);
        if (save != null)
        {
            Debug.Log("Reloaded save file");
            LevelProgressManager.Initialize(levelDB);
            save.ActivateRuntimeData();
            GameSave.activeFile = save;
            continueButton.gameObject.SetActive(true);
            return;
        }

        // fresh startup new game
        Debug.Log("New save file");
        GameSave.activeFile = new GameSave();
        LevelProgressManager.Initialize(levelDB);

        if (GameSave.activeFile.newGame)
            continueButton.gameObject.SetActive(false);

    }
}
