using System.Collections;
using System.Collections.Generic;
using CHARACTERS;
using COMMANDS;
using UnityEngine;

namespace DIALOGUE
{
    public class ConversationManager
    {
        private DialogueSystem dialogueSystem => DialogueSystem.instance;
        private Coroutine process = null;
        public bool isRunning => process != null;
        public TextArchitect archi = null;
        private bool userPrompt = false;
        public ConversationManager(TextArchitect archi)
        {
            this.archi = archi;
            dialogueSystem.onUserPromptNext += OnUserPromptNext;
        }

        private void OnUserPromptNext()
        {
            userPrompt = true;
        }

        public Coroutine StartConversation(List<string> conversation)
        {
            StopConversation();

            process = dialogueSystem.StartCoroutine(RunningConversation(conversation));
            return process;
        }

        public void StopConversation()
        {
            if (!isRunning)
                return;

            dialogueSystem.StopCoroutine(process);
            process = null;
        }

        IEnumerator RunningConversation(List<string> conversation)
        {
            for (int i = 0; i < conversation.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(conversation[i]))
                    continue;

                DIALOGUE_LINES line = DialogueParser.Parse(conversation[i]);

                //show dialogue
                if (line.hasDialogue)
                    yield return LineRunDialogue(line);

                // run commands when the dialogue FINISHED NOT AFTER THE USER CLICKED TO GO NEXT DIALOGUE
                if (line.hasCommands)
                    yield return LineRunCmds(line);

                // wait for user inout if dialogue was in this line
                if (line.hasDialogue)
                {
                    // wait for user input to go next
                    yield return WaitForUserInput();

                    CommandManager.instance.StopAllProcesses();
                }

            }
        }

        IEnumerator LineRunDialogue(DIALOGUE_LINES line)
        {
            if (line.hasSpeaker)
                HandleSpeakerLogic(line.speakerData);

            // this line makes it so that whenever the dialogue box is not showing and then speaker starts speaking some dialogue
            // it will automatically show the dialogue box again without specifying the command ShowDB() in the dialogue files
            if (!dialogueSystem.dialogueContainer.isVisible)
                dialogueSystem.dialogueContainer.Show();

            yield return BuildLineSegments(line.dialogueData);

        }

        private void HandleSpeakerLogic(DL_SPEAKER_DATA speakerData)
        {
            bool charMustBeCreated = (speakerData.makeCharEnter || speakerData.isCastingExpressions);
            Character character = CharacterManager.instance.GetCharacter(speakerData.name, createIfDoesnNotExist: charMustBeCreated);

            if (speakerData.makeCharEnter && (!character.isVisible && !character.isRevealing))
                character.Show();

            dialogueSystem.ShowSpeakerName(speakerData.displayName);
            DialogueSystem.instance.ApplySpeakerData(speakerData.name);

            // cast expression
            if (speakerData.isCastingExpressions)
            {
                foreach (var ce in speakerData.CastExpressions)
                {
                    character.OnReceiveCastingExpr(ce.layer, ce.expression);

                }
            }

        }

        IEnumerator LineRunCmds(DIALOGUE_LINES line)
        {
            List<DL_COMMAND_DATA.Command> commands = line.commandsData.commands;

            foreach (DL_COMMAND_DATA.Command command in commands)
            {
                if (command.waitForCompletion || command.name == "wait")
                {
                    CoroutineWrapper cw = CommandManager.instance.Execute(command.name, command.arguments);
                    while (!cw.isDone)
                    {
                        if (userPrompt)
                        {
                            CommandManager.instance.StopCurrentProcess();
                            userPrompt = false;
                        }
                        yield return null;
                    }
                }
                else
                    CommandManager.instance.Execute(command.name, command.arguments);
            }
            yield return null;
        }

        IEnumerator BuildLineSegments(DL_DIALOGUE_DATA line)
        {
            for (int i = 0; i < line.segments.Count; i++)
            {
                DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment = line.segments[i];

                yield return OnDialogueSegmentSignal(segment);
                yield return BuildDialogue(segment.dialogue, segment.appendText);
            }
        }

        // this is because whenever there is a {wa}(a wait) the auto reader doesnt take that into account
        // so after waiting for how long based on {wa} it would skip right next to the next dialogue
        // because the auto reader is still running
        public bool isWaitingOnAutoTimer { get; set; } = false;
        IEnumerator OnDialogueSegmentSignal(DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment)
        {
            switch (segment.startSignal)
            {
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.C:
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WC:
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WA:
                    isWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    isWaitingOnAutoTimer = false;
                    break;
                default:
                    break;
            }
        }

        IEnumerator BuildDialogue(string dialogue, bool append = false)
        {
            // build dialogue 
            if (!append)
                archi.Build(dialogue);
            else
                archi.Append(dialogue);

            // wait for dialogue to complete
            while (archi.isBuilding)
            {
                if (userPrompt)
                {
                    if (!archi.hurryUp)
                        archi.hurryUp = true;
                    else
                        archi.ForceComplete();
                    userPrompt = false;
                }
                yield return null;
            }

        }


        IEnumerator WaitForUserInput()
        {
            dialogueSystem.prompt.Show();
            while (!userPrompt)
                yield return null;

            dialogueSystem.prompt.Hide();

            userPrompt = false;
        }
    }
}
