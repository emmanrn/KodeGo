using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DIALOGUE
{
    public enum GeneralActionMap
    {
        LEVEL_MENU,
        DIALOGUE,
    }
    public class ActionMapEntry
    {
        public string name { get; set; }
        public string group { get; set; }
        public InputAction action { get; set; }
        public Action<InputAction.CallbackContext> command { get; set; }

        public ActionMapEntry(string name, InputAction action, Action<InputAction.CallbackContext> command, string group = "")
        {
            this.name = name;
            this.group = group;
            this.action = action;
            this.command = command;
        }
    }
    public class PlayerInputManager : MonoBehaviour
    {
        private PlayerInput input;
        private List<ActionMapEntry> actionMaps = new();
        public static PlayerInputManager instance;
        private HashSet<string> activeActionGroups = new();

        public event Action OnNextLevelEvent;
        public event Action OnPrevLevelEvent;
        public event Action<InputAction.CallbackContext> OnMoveEvent;
        void Awake()
        {
            //input.actions["Next"].performed += PromptAdvance;
            input = GetComponent<PlayerInput>();
            if (instance == null)
            {
                instance = this;

            }
            else
                DestroyImmediate(gameObject);
            InitializeActions();

        }

        private void InitializeActions()
        {
            // General Action Map
            //actionMaps.Add(new ActionMapEntry("General", input.actions["Next"], OnNextPrompt, group: "DIALOGUE"));
            //actionMaps.Add(new ActionMapEntry("General", input.actions["NextLevel"], OnLevelNext, group: "LEVEL_MENU"));
            //actionMaps.Add(new ActionMapEntry("General", input.actions["PrevLevel"], OnLevelPrev, group: "LEVEL_MENU"));
            //actionMaps.Add(new ActionMapEntry("General", input.actions["MenuOPEN"], OnLevelPrev));

            //Player Action Map

        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {

            for (int i = 0; i < actionMaps.Count; i++)
            {
                var inputAction = actionMaps[i];
                inputAction.action.performed -= inputAction.command;
            }
        }

        public void EnableActionMap(string actionMapName, string groupName = "", bool enableGroup = false)
        {
            input.actions.FindActionMap(actionMapName)?.Enable();

            Debug.Log("Enabled");


            if (enableGroup)
            {
                EnableGroup(actionMapName, groupName);
                return;
            }

            for (int i = 0; i < actionMaps.Count; i++)
            {
                var inputAction = actionMaps[i];
                if (actionMapName == inputAction.name)
                    SubscribeToAction(inputAction);
            }

        }

        public void DisableActionMap(string actionMapName)
        {
            if (input == null)
            {
                Debug.Log("Diaabled");
                return;

            }
            input.actions.FindActionMap(actionMapName)?.Disable();

        }

        private void EnableGroup(string actionMapName, string groupName)
        {
            DisableGroup(actionMapName);

            for (int i = 0; i < actionMaps.Count; i++)
            {
                var inputAction = actionMaps[i];

                if (groupName == inputAction.group)
                    SubscribeToAction(inputAction);
            }

        }


        private void DisableGroup(string actionMapName)
        {
            for (int i = 0; i < actionMaps.Count; i++)
            {
                var inputAction = actionMaps[i];

                if (inputAction.name == actionMapName)
                    UnsubscribeToAction(inputAction);

            }
        }

        private void SubscribeToAction(ActionMapEntry inputAction) => inputAction.action.performed += inputAction.command;
        private void UnsubscribeToAction(ActionMapEntry inputAction) => inputAction.action.performed -= inputAction.command;


        private void OnMove(InputAction.CallbackContext c) => OnMoveEvent?.Invoke(c);
        private void OnLevelNext(InputAction.CallbackContext c) => OnNextLevelEvent?.Invoke();

        private void OnLevelPrev(InputAction.CallbackContext c) => OnPrevLevelEvent?.Invoke();
        public void OnNextPrompt(InputAction.CallbackContext c) => DialogueSystem.instance.OnUserPromptNext();

        //private void OnEnable()
        //{
        //    for (int i = 0; i < actions.Count; i++)
        //    {
        //        var inputAction = actions[i];
        //        inputAction.action.performed += inputAction.command;
        //    }
        //}


        //private void OnDisable()
        //{
        //    for (int i = 0; i < actions.Count; i++)
        //    {
        //        var inputAction = actions[i];
        //        inputAction.action.performed -= inputAction.command;
        //    }
        //}
    }

}
