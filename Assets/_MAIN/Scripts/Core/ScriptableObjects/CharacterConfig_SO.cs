using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    [CreateAssetMenu(fileName = "Charcter Config Asset", menuName = "Dialogue System/Character Config Asset")]
    public class CharacterConfig_SO : ScriptableObject
    {
        public CharacterConfigData[] characters;

        public CharacterConfigData GetConfig(string characterName)
        {
            characterName = characterName.ToLower();

            for (int i = 0; i < characters.Length; i++)
            {
                CharacterConfigData data = characters[i];

                if (string.Equals(characterName, data.name.ToLower()) || string.Equals(characterName, data.alias.ToLower()))
                    // the reason we are making a copy of the config data, is because since characterconfigdaata is a scriptable object
                    // whenever you change something in the inspector while in game and then exit out of the game, it will keep the changes.
                    return data.Copy();

            }

            return CharacterConfigData.Default;
        }
    }

}