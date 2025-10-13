using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DB_EXTENSION_MAINGAME : CMD_DB_EXTENSION
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("setplayername", new Action<string>(SetPlayerNameVariable));
        }

        private static void SetPlayerNameVariable(string data)
        {
            MAIN_GAME.GameSave.activeFile.playerName = data;
        }
    }

}