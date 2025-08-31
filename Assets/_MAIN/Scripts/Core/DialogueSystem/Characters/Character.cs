using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

namespace CHARACTERS
{
    public abstract class Character
    {
        public const bool ENABLE_ON_START = false;
        public string name = "";
        public string displayName = "";
        public RectTransform root = null;
        public CharacterConfigData config;
        public Animator animator;

        public CharacterManager characterManager => CharacterManager.instance;
        public DialogueSystem dialogueSystem => DialogueSystem.instance;

        //Coroutines
        protected Coroutine CO_Revealing, CO_Hiding;
        public bool isRevealing => CO_Revealing != null;
        public bool isHiding => CO_Hiding != null;
        // this is a virtual one because it isnt going to be true for all character types like text
        public virtual bool isVisible { get; set; }

        public Character(string name, CharacterConfigData config, GameObject prefab)
        {
            this.name = name;
            displayName = name;
            this.config = config;

            if (prefab != null)
            {
                GameObject obj = Object.Instantiate(prefab, characterManager.characterPanel);
                obj.name = characterManager.FormatCharacterPath(characterManager.characterPrefabNameFormat, name);
                obj.SetActive(true);
                root = obj.GetComponent<RectTransform>();
                animator = root.GetComponentInChildren<Animator>();
            }
        }

        public Coroutine Say(string dialogue) => Say(new List<string>() { dialogue });
        public Coroutine Say(List<string> dialogue)
        {
            dialogueSystem.ShowSpeakerName(displayName);
            UpdateTxtCustomiationsOnScreen();
            return dialogueSystem.Say(dialogue);
        }

        public void SetNameColor(Color color) => config.nameColor = color;
        public void SetDialogueColor(Color color) => config.dialogueColor = color;
        public void ResetConfigData() => config = CharacterManager.instance.GetCharacterConfig(name, getOriginal: true);
        public void UpdateTxtCustomiationsOnScreen() => dialogueSystem.ApplySpeakerData(config);

        public virtual Coroutine Show()
        {
            if (isRevealing)
                characterManager.StopCoroutine(CO_Revealing);

            if (isHiding)
                characterManager.StopCoroutine(CO_Hiding);

            CO_Revealing = characterManager.StartCoroutine(ShowingOrHiding(true));
            return CO_Revealing;
        }

        public virtual Coroutine Hide()
        {
            if (isHiding)
                characterManager.StopCoroutine(CO_Hiding);

            if (isRevealing)
                characterManager.StopCoroutine(CO_Revealing);

            CO_Hiding = characterManager.StartCoroutine(ShowingOrHiding(false));
            return CO_Hiding;
        }

        public virtual IEnumerator ShowingOrHiding(bool show)
        {
            Debug.Log("Show/Hide cannot be called from a base character type");
            yield return null;
        }
        public virtual void OnReceiveCastingExpr(int layer, string expression)
        {
            return;
        }
        public enum CharacterType
        {
            Text,   // a character with no sprites on the screen, only dialogue
            Sprite, // uses textures as sprites
            Spritesheet // has multiple sprites per texture
        }
    }

}
