using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

namespace MAIN_GAME
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; }

        void Awake()
        {
            if (instance == null)
                instance = this;
            else
                DestroyImmediate(gameObject);

            GameDatabaseLinkSetup linkSetup = GetComponent<GameDatabaseLinkSetup>();
            linkSetup.SetupExternalLinks();
        }

        public void LoadFile(string filePath)
        {
            List<string> lines = new List<string>();

            TextAsset file = Resources.Load<TextAsset>(filePath);

            try
            {
                lines = FileManager.ReadTxtAsset(file);
            }
            catch
            {
                Debug.LogError($"Dialogue file at path 'Resources/{filePath}' does not exist");
                return;
            }

            DialogueSystem.instance.Say(lines, filePath);
        }
    }
}