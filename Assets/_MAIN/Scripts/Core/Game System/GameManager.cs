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

        void Awake()
        {
            if (instance == null)
                instance = this;
            else
                DestroyImmediate(gameObject);

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
            if (GameSave.activeFile != null)
                GameSave.activeFile.Activate();
            // if (GameSave.activeFile.newGame)
            // {
            //     Debug.Log(" new game");
            // }
            // else
            // {
            //     GameSave.activeFile.Activate();
            // }
        }

    }
}