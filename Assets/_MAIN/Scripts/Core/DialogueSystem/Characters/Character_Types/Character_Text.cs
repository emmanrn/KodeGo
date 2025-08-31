using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    public class Character_Text : Character
    {
        // so basically what this does is that we need to have the same constructor for this and the Character (parent) class.
        // this is basically just we define the same constructor but we are also calling the constructor from the parent Character class.
        public Character_Text(string name, CharacterConfigData config) : base(name, config, prefab: null)
        {
            Debug.Log($"Text Character: {name}");
        }
    }

}
