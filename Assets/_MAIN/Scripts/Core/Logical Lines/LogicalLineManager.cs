using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

namespace DIALOGUE.LogicalLines
{
    public class LogicalLineManager
    {
        private DialogueSystem dialogueSystem => DialogueSystem.instance;
        private List<ILogicalLine> logicalLines = new List<ILogicalLine>();

        public LogicalLineManager() => LoadLogicalLines();

        private void LoadLogicalLines()
        {
            // This is retrieving all the types in the currently executing assembly that
            // implement the `ILogicalLine` interface and are not interfaces themselves
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] lineTypes = assembly.GetTypes()
                    .Where(t => typeof(ILogicalLine).IsAssignableFrom(t) && !t.IsInterface)
                    .ToArray();


            // here we are just iterating through each type and then creating an instance of it
            // and storing that instance to the list of ILogicalLine
            // this way so that we canget the data from it and get the logic that needs to run
            foreach (Type lineType in lineTypes)
            {
                ILogicalLine line = (ILogicalLine)Activator.CreateInstance(lineType);
                logicalLines.Add(line);
            }
        }

        public bool TryGetLogic(DIALOGUE_LINES line, out Coroutine logic)
        {
            foreach (var logicalLine in logicalLines)
            {
                if (logicalLine.Matches(line))
                {
                    logic = dialogueSystem.StartCoroutine(logicalLine.Execute(line));
                    return true;
                }
            }

            logic = null;
            return false;
        }
    }

}