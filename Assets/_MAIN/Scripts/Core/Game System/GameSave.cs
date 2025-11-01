using System.Collections.Generic;
using System.Linq;
using DIALOGUE;
using HISTORY;
using UnityEngine;

namespace MAIN_GAME
{
    [System.Serializable]
    public class GameSave
    {
        public static GameSave activeFile = null;

        public const string FILE_TYPE = ".gmsv";
        public const bool ENCRYPT_FILES = true;

        public string filePath = $"{FilePaths.gameSaves}save{FILE_TYPE}";

        public string playerName;

        public bool newGame = true;
        public Game_LevelData[] levelProgress;
        public string[] activeConversations;
        public HistoryState[] historyLogs;
        public Game_VariableData[] variables;

        public static GameSave Load(string filePath, bool activateOnLoad = false)
        {
            GameSave save = FileManager.Load<GameSave>(filePath, ENCRYPT_FILES);

            activeFile = save;

            if (activateOnLoad)
                save.Activate();

            return save;
        }

        public void Save()
        {
            newGame = false;
            // we're just gonna set the default player name to "Kode" here if no playerName was given (which most likely is)
            // but just incase we made it to where the player chooses their name then we just remove this if statement
            if (string.IsNullOrEmpty(playerName))
                playerName = "Kode";

            historyLogs = HistoryManager.instance.history.ToArray();
            activeConversations = GetConversationData();
            levelProgress = GetLevelData();
            variables = GetVariableData();

            string saveJSON = JsonUtility.ToJson(this);
            FileManager.Save(filePath, saveJSON, ENCRYPT_FILES);
        }

        public void ActivateRuntimeData()
        {
            SetLevelData();
            SetVariableData();
        }

        public void Activate()
        {
            HistoryManager.instance.history = historyLogs.ToList();

            HistoryManager.instance.logManager.Clear();
            HistoryManager.instance.logManager.Rebuild();

            DialogueSystem.instance.prompt.Hide();

            // SetLevelData();
            // SetVariableData();
        }

        private string[] GetConversationData()
        {
            List<string> returnData = new List<string>();
            var conversations = DialogueSystem.instance.conversationManager.GetConversationQueue();

            for (int i = 0; i < conversations.Length; i++)
            {
                var conversation = conversations[i];
                string data = "";

                if (conversation.file != string.Empty)
                {
                    var compressedData = new Game_ConversationDataCompressed();
                    compressedData.fileName = conversation.file;
                    compressedData.progress = conversation.GetProgress();
                    compressedData.startIndex = conversation.fileStartIndex;
                    compressedData.endIndex = conversation.fileEndIndex;
                    data = JsonUtility.ToJson(compressedData);
                }
                // this is if we're not using compressed data meaning we just save the whole conversation
                else
                {
                    var fullData = new Game_ConversationData();
                    fullData.conversation = conversation.GetLines();
                    fullData.progress = conversation.GetProgress();
                    data = JsonUtility.ToJson(fullData);
                }
                returnData.Add(data);
            }
            return returnData.ToArray();
        }

        private Game_VariableData[] GetVariableData()
        {
            List<Game_VariableData> returnData = new List<Game_VariableData>();
            // getting all the variables in all the databases
            foreach (var database in VariableStore.databases.Values)
            {
                foreach (var variable in database.variables)
                {
                    Game_VariableData variableData = new Game_VariableData();
                    variableData.name = $"{database.name}.{variable.Key}";

                    // whatever value it is we convert it to string
                    string val = $"{variable.Value.Get()}";
                    variableData.value = val;
                    variableData.type = val == string.Empty ? "System.String" : variable.Value.Get().GetType().ToString();
                    returnData.Add(variableData);

                }
            }
            return returnData.ToArray();
        }

        private void SetVariableData()
        {
            foreach (var variable in variables)
            {
                string val = variable.value;

                switch (variable.type)
                {
                    case "System.Boolean":
                        if (bool.TryParse(val, out bool b_val))
                        {
                            VariableStore.TrySetValue(variable.name, b_val);
                            continue;
                        }
                        break;
                    case "System.Int32":
                        if (int.TryParse(val, out int i_val))
                        {
                            VariableStore.TrySetValue(variable.name, i_val);
                            continue;
                        }
                        break;
                    case "System.Single":
                        if (float.TryParse(val, out float f_val))
                        {
                            VariableStore.TrySetValue(variable.name, f_val);
                            continue;
                        }
                        break;
                    case "System.String":
                        VariableStore.TrySetValue(variable.name, val);
                        continue;
                }
                Debug.LogError($"Could not interpret variable type: {variable.name} = {variable.type}");

            }
        }

        private Game_LevelData[] GetLevelData()
        {
            List<Game_LevelData> returnData = new List<Game_LevelData>();

            // getting all the variables in level data from the runtime dictionary in LevelProgressManager
            foreach (var data in LevelProgressManager.runtime)
            {
                var level = data.Value;
                Game_LevelData levelData = new Game_LevelData();
                levelData.levelName = level.levelName;
                levelData.collectedBlocks = level.collectedBlocks;
                levelData.quizPassed = level.quizPassed;
                levelData.secretFound = level.secretFound;
                levelData.unlocked = level.unlocked;
                levelData.completed = level.completed;
                levelData.skinUnlocked = level.skinUnlocked;
                levelData.deathCount = level.deathCount;
                levelData.title = level.title;
                levelData.checkpoint = level.checkpoint;
                levelData.hasCheckpoint = level.hasCheckpoint;
                levelData.completionPrecent = level.completionPrecent;

                returnData.Add(levelData);
            }
            return returnData.ToArray();
        }

        private void SetLevelData()
        {
            foreach (var data in levelProgress)
            {
                if (LevelProgressManager.runtime.ContainsKey(data.levelName))
                {
                    var level = LevelProgressManager.runtime[data.levelName];
                    level.collectedBlocks = data.collectedBlocks;
                    level.quizPassed = data.quizPassed;
                    level.secretFound = data.secretFound;
                    level.completed = data.completed;
                    level.unlocked = data.unlocked;
                    level.skinUnlocked = data.skinUnlocked;
                    level.deathCount = data.deathCount;
                    level.title = data.title;
                    level.checkpoint = data.checkpoint;
                    level.hasCheckpoint = data.hasCheckpoint;
                    level.completionPrecent = data.completionPrecent;
                }
            }
        }
    }
}