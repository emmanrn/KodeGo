using System.Reflection;
using System.Linq;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using CHARACTERS;

namespace COMMANDS
{

    public class CommandManager : MonoBehaviour
    {
        private const char SUB_CMD_IDENTIFIER = '.';
        public const string DATABASE_CHARACTERS_BASE = "characters";
        public const string DATABASE_CHARACTERS_SPRITE = "characters_sprite";
        public static CommandManager instance { get; private set; }
        private CommandDatabase database;
        private Dictionary<string, CommandDatabase> subDatabases = new Dictionary<string, CommandDatabase>();
        private List<CommandProcess> activeProcesses = new List<CommandProcess>();
        private CommandProcess topProcess => activeProcesses.Last();
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                database = new CommandDatabase();

                Assembly assembly = Assembly.GetExecutingAssembly();
                // get every single db extension in the project
                Type[] extensionTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CMD_DB_EXTENSION))).ToArray();

                foreach (Type extension in extensionTypes)
                {
                    MethodInfo extendMethod = extension.GetMethod("Extend");
                    extendMethod.Invoke(null, new object[] { database });
                }
            }
            else
                DestroyImmediate(gameObject);
        }


        public CoroutineWrapper Execute(string cmdName, params string[] args)
        {
            if (cmdName.Contains(SUB_CMD_IDENTIFIER))
                return ExecuteSubCommand(cmdName, args);

            Delegate command = database.GetCommand(cmdName);

            if (command == null)
                return null;

            return StartExecution(cmdName, command, args);
        }
        private CoroutineWrapper ExecuteSubCommand(string cmdName, string[] args)
        {
            string[] parts = cmdName.Split(SUB_CMD_IDENTIFIER);
            string databaseName = string.Join(SUB_CMD_IDENTIFIER, parts.Take(parts.Length - 1));
            string subCommandName = parts.Last();

            // checking to see if the database exists
            // this part is only used for making sub databases e.g Camera.SetFOV, Scene.Load
            // which has there own sub commands
            if (subDatabases.ContainsKey(databaseName))
            {
                Delegate command = subDatabases[databaseName].GetCommand(subCommandName);
                if (command != null)
                {
                    return StartExecution(cmdName, command, args);
                }
                else
                {
                    Debug.LogError($"No command called '{subCommandName}' was found in sub database '{databaseName}'");
                }
            }

            // if we made it here and not found a sub database then, this is a character command
            // and try to execute the character command, this makes Kode.Show() possible
            // instead of Show(Kode)
            // databaseName here is now the character name
            string characterName = databaseName;
            if (CharacterManager.instance.HasCharacter(characterName))
            {
                List<string> newArgs = new List<string>(args);
                newArgs.Insert(0, characterName);
                args = newArgs.ToArray();

                return ExecuteCharacterCommand(subCommandName, args);
            }

            Debug.LogError($"No sub database called '{databaseName}' exists. Command '{subCommandName}' could not be run");
            return null;
        }

        private CoroutineWrapper ExecuteCharacterCommand(string cmdName, params string[] args)
        {
            Delegate command = null;

            // finding and assigning the functions that are available to all character types
            CommandDatabase db = subDatabases[DATABASE_CHARACTERS_BASE];
            if (db.hasCommand(cmdName))
            {
                command = db.GetCommand(cmdName);
                return StartExecution(cmdName, command, args);
            }

            // looking for character specific commands now
            // args[0] because that first element is the name of the character
            // find the character, get their config and find out what type they are
            CharacterConfigData characterConfigData = CharacterManager.instance.GetCharacterConfig(args[0]);
            switch (characterConfigData.characterType)
            {
                case Character.CharacterType.Sprite:
                case Character.CharacterType.Spritesheet:
                    db = subDatabases[DATABASE_CHARACTERS_SPRITE];
                    break;
            }

            command = db.GetCommand(cmdName);

            if (command != null)
                return StartExecution(cmdName, command, args);

            Debug.LogError($"Command Manager was unable to execute command '{cmdName}' on character '{args[0]}'");
            return null;
        }

        private CoroutineWrapper StartExecution(string commandName, Delegate command, params string[] args)
        {
            System.Guid processID = System.Guid.NewGuid();
            CommandProcess cmd = new CommandProcess(processID, commandName, command, null, args, null);
            activeProcesses.Add(cmd);


            Coroutine co = StartCoroutine(RunningProcess(cmd));

            cmd.runningProcess = new CoroutineWrapper(this, co);

            return cmd.runningProcess;
        }

        public void StopCurrentProcess()
        {
            if (topProcess != null)
                KillProcess(topProcess);

        }

        public void StopAllProcesses()
        {
            foreach (var c in activeProcesses)
            {
                if (c.runningProcess != null && !c.runningProcess.isDone)
                    c.runningProcess.Stop();

                c.onTerminateAction?.Invoke();
            }

            activeProcesses.Clear();
        }

        private IEnumerator RunningProcess(CommandProcess process)
        {
            yield return WaitingForProcesToComplete(process.command, process.args);

            KillProcess(process);
        }

        public void KillProcess(CommandProcess cmd)
        {
            activeProcesses.Remove(cmd);

            if (cmd.runningProcess != null && !cmd.runningProcess.isDone)
                cmd.runningProcess.Stop();

            cmd.onTerminateAction?.Invoke();
        }

        private IEnumerator WaitingForProcesToComplete(Delegate command, string[] args)
        {
            if (command is Action)
                command.DynamicInvoke();

            else if (command is Action<string>)
                command.DynamicInvoke(args.Length == 0 ? string.Empty : args[0]);

            else if (command is Action<string[]>)
                command.DynamicInvoke((object)args);

            else if (command is Func<IEnumerator>)
                // yield for however long this ienum is going to take basically
                yield return ((Func<IEnumerator>)command)();

            else if (command is Func<string, IEnumerator>)
                yield return ((Func<string, IEnumerator>)command)(args.Length == 0 ? string.Empty : args[0]);

            else if (command is Func<string[], IEnumerator>)
                yield return ((Func<string[], IEnumerator>)command)(args);
            // for the Func<string[], Ienum> the passsing of the args in ...command(args) <-- no need to do (object)(args) cuz the ienum is already doing it
            // so we cn just pass it directly

            // add new conditions if there are like new params and stuff
        }

        public void AddTerminateActionToCurrentProcess(UnityAction action)
        {
            CommandProcess process = topProcess;
            if (process == null)
                return;

            process.onTerminateAction = new UnityEvent();
            process.onTerminateAction.AddListener(action);
        }

        public CommandDatabase CreateSubDatabase(string name)
        {
            name = name.ToLower();

            if (subDatabases.TryGetValue(name, out CommandDatabase db))
            {
                Debug.LogWarning($"A database by the name of '{name}' already exists");
                return db;
            }

            CommandDatabase newDB = new();
            subDatabases.Add(name, newDB);

            return newDB;
        }
    }
}