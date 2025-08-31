using System;
using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DB_EXTENSION_GENERAL : CMD_DB_EXTENSION
    {
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("wait", new Func<string, IEnumerator>(Wait));
            database.AddCommand("showdb", new Func<IEnumerator>(ShowDialogueBox));
            database.AddCommand("hidedb", new Func<IEnumerator>(HideDialogueBox));

        }

        private static IEnumerator Wait(string data)
        {
            if (float.TryParse(data, out float time))
            {
                yield return new WaitForSeconds(time);
            }

        }

        private static IEnumerator ShowDialogueBox()
        {
            yield return DialogueSystem.instance.dialogueContainer.Show();
        }

        private static IEnumerator HideDialogueBox()
        {
            yield return DialogueSystem.instance.dialogueContainer.Hide();
        }
    }

}
