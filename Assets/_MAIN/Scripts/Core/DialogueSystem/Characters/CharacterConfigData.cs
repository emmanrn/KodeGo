using System.Collections;
using System.Collections.Generic;
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

        public CharacterConfigData Copy()
        {
            CharacterConfigData data = new();

            data.name = name;
            data.alias = alias;
            data.characterType = characterType;

            data.nameColor = new Color(nameColor.r, nameColor.g, nameColor.b, nameColor.a = 1);
            data.dialogueColor = new Color(dialogueColor.r, dialogueColor.g, dialogueColor.b, dialogueColor.a = 1);

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

                return data;

            }
        }

    }
}
