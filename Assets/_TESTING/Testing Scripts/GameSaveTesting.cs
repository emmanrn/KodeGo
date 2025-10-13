using System.Collections;
using System.Collections.Generic;
using MAIN_GAME;
using UnityEngine;

namespace TESTING
{
    public class GameSaveTesting : MonoBehaviour
    {
        void Start()
        {
            GameSave.activeFile = new GameSave();
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                GameSave.activeFile.Save();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                GameSave.activeFile = FileManager.Load<GameSave>($"{FilePaths.gameSaves}save{GameSave.FILE_TYPE}");
                GameSave.activeFile.Load();
            }

        }
    }
}
