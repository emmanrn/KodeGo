using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

namespace DIALOGUE
{
    public class DialogueParser
    {
        private const string cmdRegexPattern = @"[\w\[\]]*[^\s]\(";
        // we are bascially pssing in the string straight from the text file
        public static DIALOGUE_LINES Parse(string rawLine)
        {

            (string speaker, string dialogue, string commands) = RipContent(rawLine);

            return new DIALOGUE_LINES(rawLine, speaker, dialogue, commands);

        }

        private static (string, string, string) RipContent(string rawLine)
        {
            string speaker = "", dialogue = "", commands = "";

            int dialogueStart = -1;
            int dialogueEnd = -1;
            bool isEscaped = false;

            for (int i = 0; i < rawLine.Length; i++)
            {
                char current = rawLine[i];

                if (current == '\\')
                    isEscaped = !isEscaped;
                else if (current == '"' && !isEscaped)
                {
                    if (dialogueStart == -1)
                        dialogueStart = i;
                    else if (dialogueEnd == -1)
                    {
                        dialogueEnd = i;
                    }
                }
                else
                    isEscaped = false;
            }

            // time to identify command expressions
            Regex cmdRegex = new Regex(cmdRegexPattern);
            MatchCollection matches = cmdRegex.Matches(rawLine);
            int cmdStart = -1;

            foreach (Match match in matches)
            {
                if (match.Index < dialogueStart || match.Index > dialogueEnd)
                {
                    cmdStart = match.Index;
                    break;
                }
            }

            if (cmdStart != -1 && dialogueStart == -1 && dialogueEnd == -1)
                return ("", "", rawLine.Trim());

            // this basically means if we found a diaaalogue and then after the dialogue is some commands
            if (dialogueStart != -1 && dialogueEnd != -1 && (cmdStart == -1 || cmdStart > dialogueEnd))
            {
                speaker = rawLine.Substring(0, dialogueStart).Trim();
                dialogue = rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1).Replace("\\\"", "\"");
                if (cmdStart != -1)
                {
                    commands = rawLine.Substring(cmdStart).Trim();
                }
            }
            // otherwise we have found a command line
            else if (cmdStart != -1 && dialogueStart > cmdStart)
                commands = rawLine;
            // or if none then we have just found a speaker, just a speaker lmao
            else
                dialogue = rawLine;




            return (speaker, dialogue, commands);

        }
    }
}
