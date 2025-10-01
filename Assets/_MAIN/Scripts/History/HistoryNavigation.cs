using System.Collections.Generic;
using DIALOGUE;
using TMPro;
using UnityEngine;

namespace HISTORY
{
    // THIS NAVIGATION WONT DO ANYTHING SINCE I DIDNT BIND IT TO A KEY YET FOR THE NEW INPUT SYSTEM
    // gonna implement this anyways cuz why not im a dumbass
    public class HistoryNavigation : MonoBehaviour
    {
        public int progress = 0;

        [SerializeField] private TextMeshProUGUI statusTxt;

        HistoryManager manager => HistoryManager.instance;

        List<HistoryState> history => manager.history;
        HistoryState cachedState = null;
        private bool isOnCachedState = false;

        public bool isViewingHistory = false;
        public void GoForward()
        {
            if (!isViewingHistory)
                return;

            HistoryState state = null;

            if (history.Count == 0 || (progress < history.Count - 1))
            {
                progress++;

                state = history[progress];
            }
            else
            {
                isOnCachedState = true;
                state = cachedState;
            }

            state.Load();

            if (isOnCachedState)
            {
                isViewingHistory = false;
                DialogueSystem.instance.onUserPromptNext -= GoForward;
                statusTxt.text = "";
                DialogueSystem.instance.OnStopViewingHistory();
            }
            else
                UpdateStatusText();
        }

        public void GoBack()
        {
            if (progress == 0 && isViewingHistory)
                return;

            // if we're viewing the history we need to decrease the progress amount and go back towards the beginning
            // if not then we need to make sure that we start at the latest point
            progress = isViewingHistory ? progress - 1 : history.Count - 1;

            if (!isViewingHistory)
            {
                isViewingHistory = true;
                isOnCachedState = false;

                cachedState = HistoryState.Capture();

                // this will make it so that when we are viewing the history
                // when we press the continuation prompt btn we wont move through the dialogue lines, but move through the states
                DialogueSystem.instance.onUserPromptNext += GoForward;
                DialogueSystem.instance.OnStartViewingHistory();
            }

            HistoryState state = history[progress];
            state.Load();
            UpdateStatusText();

        }

        private void UpdateStatusText()
        {
            statusTxt.text = $"{history.Count - progress} / {history.Count}";
        }
    }

}
