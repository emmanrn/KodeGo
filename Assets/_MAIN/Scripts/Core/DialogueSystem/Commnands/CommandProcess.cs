using UnityEngine;
using System;
using UnityEngine.Events;
using JetBrains.Annotations;

namespace COMMANDS
{
    // this alongside the coroutine wrapper is for the purpose that whenever there's an action in the cutscene the player wants to skip
    // we can keep track of the command that is running and will be used to monitor for key presses
    public class CommandProcess
    {
        public Guid ID;
        public string processName;
        public Delegate command;
        public CoroutineWrapper runningProcess;
        public string[] args;

        public UnityEvent onTerminateAction;

        public CommandProcess(Guid ID, string processName, Delegate command, CoroutineWrapper runningProcess, string[] args, UnityEvent onTerminateAction = null)
        {
            this.ID = ID;
            this.processName = processName;
            this.command = command;
            this.runningProcess = runningProcess;
            this.args = args;
            this.onTerminateAction = onTerminateAction;
        }


    }
}
