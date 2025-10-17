using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAIN_GAME
{
    public class GameDatabaseLinkSetup : MonoBehaviour
    {
        public void SetupExternalLinks()
        {
            VariableStore.CreateVariable("Main.mainCharName", "", () => GameSave.activeFile.playerName, value => GameSave.activeFile.playerName = value);
        }
    }
}
