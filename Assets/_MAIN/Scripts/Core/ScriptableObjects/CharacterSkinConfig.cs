using System.Collections;
using System.Collections.Generic;
using HISTORY;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Skin", menuName = "Character Skin Config")]
public class CharacterSkinConfig : ScriptableObject
{
    public CharacterSkin[] skins;
}
