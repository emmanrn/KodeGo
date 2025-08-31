using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{

    public class CommandDatabase
    {
        private Dictionary<string, Delegate> database = new();

        public bool hasCommand(string cmdName) => database.ContainsKey(cmdName.ToLower());

        public void AddCommand(string cmdName, Delegate command)
        {
            cmdName = cmdName.ToLower();
            if (!database.ContainsKey(cmdName))
                database.Add(cmdName, command);
            else
                Debug.Log($"Command '{cmdName}' already exists in the database");
        }

        public Delegate GetCommand(string cmdName)
        {
            cmdName = cmdName.ToLower();
            if (!database.ContainsKey(cmdName))
            {
                Debug.LogError($"Command '{cmdName}' does not exist in the database");
                return null;
            }

            return database[cmdName];
        }
    }
}