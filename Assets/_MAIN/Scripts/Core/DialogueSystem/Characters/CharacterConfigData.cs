using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DIALOGUE;
using Microsoft.SqlServer.Server;
using TMPro;
using UnityEngine;

namespace CHARACTERS
{
    [System.Serializable]
    public class CharacterConfigData
    {
        public string name;
        public string alias;
        public Character.CharacterType characterType;

        public Color nameColor;
        public Color dialogueColor;

        public float nameFontScale = 1f;
        public float dialogueFontScale = 1f;

        [SerializedDictionary("Path / ID", "Sprite")]
        public SerializedDictionary<string, Sprite> sprites = new SerializedDictionary<string, Sprite>();

        public CharacterConfigData Copy()
        {
            CharacterConfigData data = new();

            data.name = name;
            data.alias = alias;
            data.characterType = characterType;

            data.nameColor = new Color(nameColor.r, nameColor.g, nameColor.b, nameColor.a = 1);
            data.dialogueColor = new Color(dialogueColor.r, dialogueColor.g, dialogueColor.b, dialogueColor.a = 1);

            data.dialogueFontScale = dialogueFontScale;
            data.nameFontScale = nameFontScale;

            return data;
        }

        private static Color defaultColor => DialogueSystem.instance.config.defaultTxtColor;

        public static CharacterConfigData Default
        {
            get
            {
                CharacterConfigData data = new();

                data.name = "";
                data.alias = "";
                data.characterType = Character.CharacterType.Text;

                data.nameColor = defaultColor;
                data.dialogueColor = defaultColor;

                data.dialogueFontScale = 1f;
                data.nameFontScale = 1f;

                return data;

            }
        }

    }
}
