using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

namespace HISTORY
{
    [RequireComponent(typeof(HistoryLogManager))]
    [RequireComponent(typeof(HistoryNavigation))]
    public class HistoryManager : MonoBehaviour
    {
        public const int HISTORY_CACHE_LIMIT = 100;
        public static HistoryManager instance { get; private set; }
        public List<HistoryState> history = new List<HistoryState>();

        private HistoryNavigation navigation;
        public HistoryLogManager logManager { get; private set; }
        void Awake()
        {
            instance = this;
            navigation = GetComponent<HistoryNavigation>();
            logManager = GetComponent<HistoryLogManager>();
        }
        void Start()
        {
            DialogueSystem.instance.onClear += LogCurrentState;
        }

        public void LogCurrentState()
        {
            HistoryState state = HistoryState.Capture();
            history.Add(state);
            logManager.AddLog(state);

            if (history.Count > HISTORY_CACHE_LIMIT)
                history.RemoveAt(0);
        }

        public void LoadState(HistoryState state)
        {
            state.Load();
        }

        // this is also wont do anything as it is not binded to the new input system yet
        public void GoForward() => navigation.GoForward();
        public void GoBack() => navigation.GoBack();

    }

}