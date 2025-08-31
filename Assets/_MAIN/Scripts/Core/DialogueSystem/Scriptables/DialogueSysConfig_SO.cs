using UnityEngine;
using CHARACTERS;

namespace DIALOGUE
{
    [CreateAssetMenu(fileName = "Dialogue System Config", menuName = "Dialogue System/Dialogue Config Asset")]
    public class DialogueSysConfig_SO : ScriptableObject
    {
        public CharacterConfig_SO characterConfigAsset;

        public Color defaultTxtColor = Color.white;
    }
}
