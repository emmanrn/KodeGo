using System;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DIALOGUE
{
    public enum GeneralActionMap
    {
        LEVEL_MENU,
        DIALOGUE,
    }
    public class PlayerInputManager : MonoBehaviour
    {
        private PlayerInput input;
        private List<(InputAction action, Action<InputAction.CallbackContext> command)> actions = new List<(InputAction action, Action<InputAction.CallbackContext> command)>();
        public static PlayerInputManager instance;
        private HashSet<string> activeActionGroups = new();

        public event Action OnNextLevelEvent;
        public event Action OnPrevLevelEvent;
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
            actions.Add((input.actions["Next"], OnNextPrompt));
            actions.Add((input.actions["NextLevel"], OnLevelNext));
            actions.Add((input.actions["PrevLevel"], OnLevelPrev));

        }

        public void EnablePlayerMovement()
        {
            Debug.Log("Enabled Movement");
            input.actions.FindActionMap("Player")?.Enable();
            input.actions.FindActionMap("General")?.Disable();

            Unsubscribe();
        }

        public void EnableGeneral(GeneralActionMap subState)
        {
            input.actions.FindActionMap("Player")?.Disable();

            var generalActionMap = input.actions.FindActionMap("General");
            if (generalActionMap == null)
                return;

            generalActionMap.Enable();
            foreach (var (action, _) in actions)
                action?.Disable();

            Unsubscribe();

            switch (subState)
            {
                case GeneralActionMap.DIALOGUE:
                    SubscribeToAction("Next", OnNextPrompt);
                    break;
                case GeneralActionMap.LEVEL_MENU:
                    SubscribeToAction("NextLevel", OnLevelNext);
                    SubscribeToAction("PrevLevel", OnLevelPrev);
                    break;

            }
        }

        private void SubscribeToAction(string actionName, Action<InputAction.CallbackContext> callback)
        {
            var inputActions = actions.Find(a => a.action?.name == actionName);

            if (inputActions.action != null)
            {
                inputActions.action.Enable();
                inputActions.action.performed += callback;
            }
        }


        private void OnEnable()
        {
            InitializeActions();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                var inputAction = actions[i];
                inputAction.action.performed -= inputAction.command;
            }

        }

        public void EnableGroup(string groupName)
        {
            activeActionGroups.Clear();
            activeActionGroups.Add(groupName);

            Unsubscribe();
        }

        // if we want to have multiple actions groups enabled e.g dialogue + level menu
        private void AddGroup(string groupName)
        {
            activeActionGroups.Add(groupName);
            Unsubscribe();
        }

        public void UnsubscribeAll()
        {
            activeActionGroups.Clear();
            Unsubscribe();
        }

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
