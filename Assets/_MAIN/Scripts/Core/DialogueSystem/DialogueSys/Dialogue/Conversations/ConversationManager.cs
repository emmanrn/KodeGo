using System.Collections;
using System.Collections.Generic;
using CHARACTERS;
using COMMANDS;
using DIALOGUE.LogicalLines;
using Unity.VisualScripting;
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


        private LogicalLineManager logicalLineManager;
        public Conversation conversation => (conversationQueue.isEmpty() ? null : conversationQueue.top);
        public int conversationPorgress => (conversationQueue.isEmpty() ? -1 : conversationQueue.top.GetProgress());
        private ConversationQueue conversationQueue;

        public bool allowUserPrompts = true;


        public ConversationManager(TextArchitect archi)
        {
            this.archi = archi;
            dialogueSystem.onUserPromptNext += OnUserPromptNext;

            logicalLineManager = new LogicalLineManager();
            conversationQueue = new ConversationQueue();
        }

        public void Enqueue(Conversation conversation) => conversationQueue.Enqueue(conversation);
        public void EnqueuePriority(Conversation conversation) => conversationQueue.EnqueuePriority(conversation);

        private void OnUserPromptNext()
        {
            if (allowUserPrompts)
                userPrompt = true;
        }

        public Coroutine StartConversation(Conversation conversation)
        {
            StopConversation();
            conversationQueue.Clear();

            Enqueue(conversation);

            process = dialogueSystem.StartCoroutine(RunningConversation());
            return process;
        }

        public void StopConversation()
        {
            if (!isRunning)
                return;

            dialogueSystem.StopCoroutine(process);

            process = null;
        }

        IEnumerator RunningConversation()
        {
            while (!conversationQueue.isEmpty())
            {
                Conversation currentConversation = conversation;

                if (currentConversation.HasReachedEnd())
                {
                    conversationQueue.Dequeue();
                    continue;
                }

                string rawLine = currentConversation.CurrentLine();

                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    TryAdvanceCurrentConverstion(currentConversation);
                    continue;
                }

                DIALOGUE_LINES line = DialogueParser.Parse(rawLine);

                if (logicalLineManager.TryGetLogic(line, out Coroutine logic))
                {
                    yield return logic;
                }
                else
                {
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

                        dialogueSystem.OnSystemPromptClear();
                    }
                }


                TryAdvanceCurrentConverstion(currentConversation);

            }
            process = null;
        }

        private void TryAdvanceCurrentConverstion(Conversation conversation)
        {
            conversation.IncrementProgress();

            if (conversation != conversationQueue.top)
                return;

            if (conversation.HasReachedEnd())
                conversationQueue.Dequeue();
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

            // add character name to the ui
            dialogueSystem.ShowSpeakerName(TagManager.Inject(speakerData.displayName));

            // customize the dialogue for this specific character's conig
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
                    yield return WaitForUserInput();
                    dialogueSystem.OnSystemPromptClear();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WC:
                    isWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    isWaitingOnAutoTimer = false;
                    dialogueSystem.OnSystemPromptClear();
                    break;
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
            // this will inject the tags into the dialogue
            dialogue = TagManager.Inject(dialogue);
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
