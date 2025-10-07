using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Encapsulation;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Conditions;

namespace DIALOGUE.LogicalLines
{
    public class LL_Condition : ILogicalLine
    {
        public string keyword => "if";
        private const string ELSE = "else";
        private readonly string[] CONTAINERS = new string[] { "(", ")" };

        public IEnumerator Execute(DIALOGUE_LINES line)
        {
            string rawCondition = ExtractCondition(line.rawData.Trim());
            bool conditionResult = EvaluateCondition(rawCondition);

            Conversation currentConversation = DialogueSystem.instance.conversationManager.conversation;
            int currentProgress = DialogueSystem.instance.conversationManager.conversationPorgress;

            // getting the if data
            // ripHeaderandEncapsulator is false because we dont need to rip a title for this one since it's just an if statement
            // we only need the lines here
            EncapsulatedData ifData = RipEncapsulationData(currentConversation, currentProgress, ripHeaderAndEncapsulators: false);
            EncapsulatedData elseData = new EncapsulatedData();

            if (ifData.endingIndex + 1 < currentConversation.Count)
            {
                string nextLine = currentConversation.GetLines()[ifData.endingIndex + 1].Trim();
                if (nextLine == ELSE)
                {
                    elseData = RipEncapsulationData(currentConversation, ifData.endingIndex + 1, false);
                    ifData.endingIndex = elseData.endingIndex;
                }
            }

            currentConversation.SetProgress(ifData.endingIndex);
            EncapsulatedData selectedData = conditionResult ? ifData : elseData;

            if (!selectedData.isNull && selectedData.lines.Count > 0)
            {
                Conversation newConversation = new Conversation(selectedData.lines);
                DialogueSystem.instance.conversationManager.EnqueuePriority(newConversation);
            }

            yield return null;
        }

        public bool Matches(DIALOGUE_LINES line)
        {
            return line.rawData.Trim().StartsWith(keyword);
        }

        private string ExtractCondition(string line)
        {
            int startIndex = line.IndexOf(CONTAINERS[0]) + 1;
            int endIndex = line.IndexOf(CONTAINERS[1]);

            return line.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}
