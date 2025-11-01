using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

namespace MAIN_GAME
{
    [RequireComponent(typeof(GameDatabaseLinkSetup))]
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; }
        public LevelDatabase_SO levelDB;
        public string LEVEL_NAME = "Level1";
        public string titleToBeUnlocked = "Level1";
        public TextAsset fileToRead;

        void Awake()
        {
            instance = this;

            if (!TryGetComponent<GameDatabaseLinkSetup>(out var linkSetup))
                Debug.Log("is null");
            linkSetup.SetupExternalLinks();

            if (GameSave.activeFile == null)
            {
                GameSave.activeFile = new GameSave();
                LevelProgressManager.Initialize(levelDB);
            }
        }

        void Start()
        {
            LoadGame();
        }

        private void LoadGame()
        {
            // if (GameSave.activeFile.newGame)
            //     return;

            // if (!GameSave.activeFile.newGame)
            //     GameSave.activeFile.Activate();
            if (GameSave.activeFile.newGame)
            {
                Debug.Log(" new game");
                // LevelProgressManager.Initialize(levelDB);
            }
            else
            {
                Debug.Log("activated load game");
                GameSave.activeFile.Activate();
            }
        }

    }
}