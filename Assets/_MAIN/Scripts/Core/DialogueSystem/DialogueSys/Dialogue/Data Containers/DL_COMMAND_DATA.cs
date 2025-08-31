using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DIALOGUE
{

    public class DL_COMMAND_DATA
    {
        public List<Command> commands;
        private const char COMMAND_SPLITTER_ID = ',';
        private const char AGRS_CONTAINER_ID = '(';
        private const string WAITCMD_ID = "[wait]";
        public struct Command
        {
            public string name;
            public string[] arguments;
            public bool waitForCompletion;
        }

        public DL_COMMAND_DATA(string rawCmd)
        {
            commands = RipCommands(rawCmd);
        }

        public List<Command> RipCommands(string rawCmd)
        {
            string[] data = rawCmd.Split(COMMAND_SPLITTER_ID, System.StringSplitOptions.RemoveEmptyEntries);
            List<Command> res = new();

            foreach (string cmd in data)
            {
                Command command = new();
                // finding the index of the parenthesis
                int index = cmd.IndexOf(AGRS_CONTAINER_ID);
                command.name = cmd.Substring(0, index).Trim();

                if (command.name.ToLower().StartsWith(WAITCMD_ID))
                {
                    // start at the index on where the actual command name starts
                    command.name = command.name.Substring(WAITCMD_ID.Length);
                    command.waitForCompletion = true;
                }
                else
                {
                    command.waitForCompletion = false;
                }

                command.arguments = GetArgs(cmd.Substring(index + 1, cmd.Length - index - 2));
                res.Add(command);
            }
            return res;
        }

        private string[] GetArgs(string args)
        {
            List<string> argsList = new();
            StringBuilder currentArg = new();
            bool inQuotes = false;

            for (int i = 0; i < args.Length; i++)
            {
                // if we found a quote for the args of the command
                if (args[i] == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                // if we are not in quotes and found a space we are at the end of the args name
                if (!inQuotes && args[i] == ' ')
                {
                    argsList.Add(currentArg.ToString());
                    currentArg.Clear();
                    continue;
                }

                // keep adding to the string build if we havent found a quote or a space
                currentArg.Append(args[i]);
            }

            if (currentArg.Length > 0)
                argsList.Add(currentArg.ToString());

            return argsList.ToArray();
        }
    }
}