using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    [CreateAssetMenu(fileName = "Charcter Config Asset", menuName = "Dialogue System/Character Config Asset")]
    public class CharacterConfig_SO : ScriptableObject
    {
        public CharacterConfigData[] characters;

        public CharacterConfigData GetConfig(string characterName, bool safe = true)
        {
            characterName = characterName.ToLower();

            for (int i = 0; i < characters.Length; i++)
            {
                CharacterConfigData data = characters[i];

                if (string.Equals(characterName, data.name.ToLower()) || string.Equals(characterName, data.alias.ToLower()))
                    // the reason we are making a copy of the config data, is because since characterconfigdaata is a scriptable object
                    // whenever you change something in the inspector while in game and then exit out of the game, it will keep the changes.
                    //
                    // NEW EDIT: so we now have a safe bool here to see if we are editing a copy or the original
                    // whenever we load the character in the game we are just going to be editing the copy
                    // BUT when we are loading up the sprites for the helper script to help us load the script the we say it's unsafe
                    // and load the original
                    return safe ? data.Copy() : data;

            }

            return CharacterConfigData.Default;
        }
    }

}