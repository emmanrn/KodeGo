
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using static DIALOGUE.LogicalLines.LogicalLineUtils.Encapsulation;

namespace DIALOGUE.LogicalLines
{
    // control wht hppen when a choice happen in a dialogue line
    public class LL_Choice : ILogicalLine
    {
        public string keyword => "choice";
        private const char CHOICE_IDENTIFIER = '-';

        public IEnumerator Execute(DIALOGUE_LINES line)
        {
            var currentConversation = DialogueSystem.instance.conversationManager.conversation;
            var progress = DialogueSystem.instance.conversationManager.conversationPorgress;
            EncapsulatedData data = RipEncapsulationData(currentConversation, progress, ripHeaderAndEncapsulators: true, parentStartingIndex: currentConversation.fileStartIndex);
            List<Choice> choices = GetChoicesFromData(data);

            string title = line.dialogueData.rawData;
            ChoicePanel panel = ChoicePanel.instance;
            string[] choiceTitles = choices.Select(c => c.title).ToArray();

            panel.Show(title, choiceTitles);

            while (panel.isWaitingOnUserChoice)
                yield return null;

            Choice selectedChoice = choices[panel.lastDecision.answerIndex];

            Conversation newConversation = new Conversation(selectedChoice.resultLines, file: currentConversation.file, fileStartIndex: selectedChoice.startIndex, fileEndIndex: selectedChoice.endIndex);

            // once the choice ends and the dialogue continues, then we continue back at the last line
            DialogueSystem.instance.conversationManager.conversation.SetProgress(data.endingIndex - currentConversation.fileStartIndex);
            DialogueSystem.instance.conversationManager.EnqueuePriority(newConversation);

            AutoReader autoReader = DialogueSystem.instance.autoReader;
            if (autoReader != null && autoReader.isOn && autoReader.skip)
            {
                if (Game_Configuration.activeConfig != null && !Game_Configuration.activeConfig.continueSkippingAfterChoice)
                    autoReader.Disable();
            }
        }


        public bool Matches(DIALOGUE_LINES line)
        {
            return (line.hasSpeaker && line.speakerData.name.ToLower() == keyword);
        }

        private List<Choice> GetChoicesFromData(EncapsulatedData data)
        {
            List<Choice> choices = new List<Choice>();
            int encapsulationDepth = 0;
            bool isFirstChoice = true;

            Choice choice = new Choice
            {
                title = string.Empty,
                resultLines = new List<string>()
            };

            int choiceIndex = 0, i = 0;
            for (i = 1; i < data.lines.Count; i++)
            {
                var line = data.lines[i];
                if (IsChoiceStart(line) && encapsulationDepth == 1)
                {
                    if (!isFirstChoice)
                    {
                        choice.startIndex = data.startingIndex + (choiceIndex + 1);
                        choice.endIndex = data.startingIndex + (i - 1);
                        choices.Add(choice);
                        choice = new Choice
                        {
                            title = string.Empty,
                            resultLines = new List<string>()
                        };
                    }

                    // right now we are assigning the title of the choice and removing the identifier of the title
                    // which is the '-'
                    choiceIndex = i;
                    choice.title = line.Trim().Substring(1);
                    isFirstChoice = false;
                    continue;
                }

                AddLineToResults(line, ref choice, ref encapsulationDepth);
            }

            // this is just adding the final choice here after the loop
            // the reason this happens is because in the loop in the final line after it exits the loop
            // it never goes back to the loop to add the final line to the list
            if (!choices.Contains(choice))
            {
                choice.startIndex = data.startingIndex + (choiceIndex + 1);
                choice.endIndex = data.startingIndex + (i - 2);
                choices.Add(choice);
            }

            return choices;
        }

        private void AddLineToResults(string line, ref Choice choice, ref int encapsulationDepth)
        {
            line.Trim();

            if (IsEncapsulationStart(line))
            {
                if (encapsulationDepth > 0)
                    choice.resultLines.Add(line);
                encapsulationDepth++;
                return;
            }

            if (IsEncapsulationEnd(line))
            {
                encapsulationDepth--;

                // this is basicaly saying that if we reach an encapsulation end but theres still depth
                // then we just basically found a sub encapsulation that just closed
                if (encapsulationDepth > 0)
                    choice.resultLines.Add(line);

                return;
            }

            choice.resultLines.Add(line);
        }

        private bool IsChoiceStart(string line) => line.Trim().StartsWith(CHOICE_IDENTIFIER);


        private struct Choice
        {
            public string title;
            public List<string> resultLines;
            public int startIndex;
            public int endIndex;
        }
    }
}