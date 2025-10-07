using UnityEngine;
using CHARACTERS;

namespace DIALOGUE
{
    [CreateAssetMenu(fileName = "Dialogue System Config", menuName = "Dialogue System/Dialogue Config Asset")]
    public class DialogueSysConfig_SO : ScriptableObject
    {
        public const float DEFAULT_FONTSIZE_DIALOGUE = 36f;
        public const float DEFAULT_FONTSIZE_NAME = 40;
        public CharacterConfig_SO characterConfigAsset;

        public Color defaultTxtColor = Color.white;
        public float dialogueFontScale = 1f;
        public float defaultDialogueFontSize = DEFAULT_FONTSIZE_DIALOGUE;
        public float defaultNameFontSize = DEFAULT_FONTSIZE_NAME;
    }
}
