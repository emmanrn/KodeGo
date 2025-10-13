using System;
using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DB_EXTENSION_GENERAL : CMD_DB_EXTENSION
    {
        private static string[] PARAM_SPEED => new string[] { "-spd", "-speed" };
        private static string[] PARAM_IMMEDIATE => new string[] { "-i", "-immediate" };
        private static string[] PARAM_FILEPATH => new string[] { "-f", "-file", "-filepath" };
        private static string[] PARAM_ENQUEUE => new string[] { "-e", "-enqueue" };
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("wait", new Func<string, IEnumerator>(Wait));

            // Dialogue System Controls
            database.AddCommand("showui", new Func<string[], IEnumerator>(ShowDialogueSystem));
            database.AddCommand("hideui", new Func<string[], IEnumerator>(HideDialogueSystem));
            database.AddCommand("stopdialogue", new Action(StopDialogue));

            // Dialogue Box Controls
            database.AddCommand("showdb", new Func<string[], IEnumerator>(ShowDialogueBox));
            database.AddCommand("hidedb", new Func<string[], IEnumerator>(HideDialogueBox));

            // Load new dialogue file command
            database.AddCommand("load", new Action<string[]>(LoadNewDialogueFile));


        }


        // added this just incase if we want to load new dialogue files for dynamic story paths
        // wont be using this but just in case we need something for this then this is already here
        // this will also check or handle if we still have dialogue left in the file but we already going to load a different file
        // it will make sure that all the lines in the dialogue file finishes before moving on to the next file
        private static void LoadNewDialogueFile(string[] data)
        {
            string fileName = string.Empty;
            bool enqueue = false;

            var parameters = ConvertDataToParams(data);

            parameters.TryGetValue(PARAM_FILEPATH, out fileName);
            parameters.TryGetValue(PARAM_ENQUEUE, out enqueue, defaultVal: false);

            // load the file from Resources
            string filePath = FilePaths.GetPathToResource(FilePaths.resources_testDialogueFiles, fileName);
            TextAsset file = Resources.Load<TextAsset>(filePath);

            if (file == null)
            {
                Debug.LogWarning($"File from {filePath} could not be loaded from dialogue files. Ensure it exists within the '{FilePaths.resources_dialogueFiles}' Resources folder");
                return;
            }

            List<string> lines = FileManager.ReadTxtAsset(file, includeBlankLines: true);
            Conversation newConversation = new Conversation(lines);

            if (enqueue)
                // this is so if we have remaining lines, then play out the remaining lines first before going to the next file
                DialogueSystem.instance.conversationManager.Enqueue(newConversation);
            else
                // this makes it so even if we have remaining lines in the current dialogue file we are on, we skip and go straight to the new file
                DialogueSystem.instance.conversationManager.StartConversation(newConversation);
        }
        private static IEnumerator Wait(string data)
        {
            if (float.TryParse(data, out float time))
            {
                yield return new WaitForSeconds(time);
            }

        }

        private static IEnumerator ShowDialogueBox(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParams(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultVal: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultVal: false);

            yield return DialogueSystem.instance.dialogueContainer.Show(speed, immediate);
        }

        private static IEnumerator HideDialogueBox(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParams(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultVal: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultVal: false);

            yield return DialogueSystem.instance.dialogueContainer.Hide(speed, immediate);
        }

        private static IEnumerator ShowDialogueSystem(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParams(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultVal: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultVal: false);

            yield return DialogueSystem.instance.Show(speed, immediate);
        }

        private static IEnumerator HideDialogueSystem(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParams(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultVal: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultVal: false);

            yield return DialogueSystem.instance.Hide(speed, immediate);
        }

        private static void StopDialogue()
        {
            DialogueSystem.instance.StopDialogue();
            Debug.Log("top dilogue");
        }
    }

}
