using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using Unity.VisualScripting;
using UnityEngine;

public class TestConversation : MonoBehaviour
{
    [SerializeField] private TextAsset fileToRead = null;
    void Start()
    {

        StartConversation();

    }

    private void StartConversation()
    {
        List<string> lines = FileManager.ReadTxtAsset(fileToRead);


        //foreach (string line in lines)
        //{
        //    if (string.IsNullOrWhiteSpace(line))
        //        continue;

        //    DIALOGUE_LINES dl = DialogueParser.Parse(line);

        //    for (int i = 0; i < dl.commandsData.commands.Count; i++)
        //    {
        //        DL_COMMAND_DATA.Command command = dl.commandsData.commands[i];
        //        Debug.Log($"Command[{i}] '{command.name}' has args: [{string.Join(", ", command.arguments)}]");
        //    }
        //}

        DialogueSystem.instance.Say(lines);


    }
}
