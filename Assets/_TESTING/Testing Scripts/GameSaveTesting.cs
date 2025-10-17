using System.Collections;
using System.Collections.Generic;
using MAIN_GAME;
using UnityEngine;

namespace TESTING
{
    public class GameSaveTesting : MonoBehaviour
    {
        public GameSave save;
        void Start()
        {
            if (GameSave.activeFile == null)
                GameSave.activeFile = new GameSave();
            else
                save = GameSave.activeFile;
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                GameSave.activeFile.Save();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                save = GameSave.Load($"{FilePaths.gameSaves}save{GameSave.FILE_TYPE}", activateOnLoad: false);
                GameSave.activeFile = save;

            }

        }
    }
}
