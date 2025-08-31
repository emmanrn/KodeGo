using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

namespace CHARACTERS
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager instance { get; private set; }
        private Dictionary<string, Character> characters = new();

        private CharacterConfig_SO config => DialogueSystem.instance.config.characterConfigAsset;
        private const string CHARACTER_CASTING_ID = " as ";
        // the reason for defining these paths in here is so that it can be changed easily in the future.
        private const string CHARACTER_NAME_ID = "<charname>";
        public string characterRootPathFormat => $"Characters/{CHARACTER_NAME_ID}";
        public string characterPrefabNameFormat => $"CharacterIcon - [{CHARACTER_NAME_ID}]";
        public string characterPrefabPathFormat => $"{characterRootPathFormat}/{characterPrefabNameFormat}";
        [SerializeField] private RectTransform _characterPanel = null;
        public RectTransform characterPanel => _characterPanel;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
                DestroyImmediate(gameObject);
        }

        public CharacterConfigData GetCharacterConfig(string characterName, bool getOriginal = false)
        {
            // the reason why we are checking if we are getting from the original is because what we do here is get another copy of the original config
            // thats why we need to check if we want to get the character's original config or its configured config
            // this is useful when the character is a spritesheet and we are casting it as something else
            // e.g Monk as Generic then we follow up with Monk.Show(), it wouldn't recognize the show because Monk isn't a character because it cannot find the name
            // from the original character configs
            if (!getOriginal)
            {
                Character character = GetCharacter(characterName);
                // if we already have the character on the scene then we'll grab the config from that existing character
                if (character != null)
                    return character.config;

            }

            // otherwise we grab the config from the original asset which is universal
            return config.GetConfig(characterName);
        }

        public Character GetCharacter(string characterName, bool createIfDoesnNotExist = false)
        {
            if (characters.ContainsKey(characterName.ToLower()))
                return characters[characterName.ToLower()];
            else if (createIfDoesnNotExist)
                return CreateCharacter(characterName);

            return null;
        }
        public Character CreateCharacter(string characterName, bool revealAtCreated = false)
        {
            if (characters.ContainsKey(characterName.ToLower()))
            {
                Debug.LogWarning($"Character '{characterName}' already exists. Did not create character");
                return null;
            }

            CHARACTER_INFO info = GetCharacterInfo(characterName);
            Character character = CreateCharacterFromInfo(info);

            characters.Add(characterName.ToLower(), character);

            if (revealAtCreated)
                character.Show();

            return character;
        }

        public bool HasCharacter(string characterName) => characters.ContainsKey(characterName.ToLower());

        private CHARACTER_INFO GetCharacterInfo(string characterName)
        {
            CHARACTER_INFO res = new();

            string[] nameData = characterName.Split(CHARACTER_CASTING_ID, System.StringSplitOptions.RemoveEmptyEntries);

            res.name = nameData[0];

            if (res.name.ToLower() == "narrator")
            {
                res.castingName = string.Empty;
            }
            else
                res.castingName = nameData.Length > 1 ? nameData[1] : res.name;



            res.config = config.GetConfig(res.castingName);
            res.prefab = GetPrefabForCharacter(res.castingName);
            res.rootCharacterFolder = FormatCharacterPath(characterRootPathFormat, res.castingName);

            return res;
        }

        private GameObject GetPrefabForCharacter(string characterName)
        {
            string prefabPath = FormatCharacterPath(characterPrefabPathFormat, characterName);
            return Resources.Load<GameObject>(prefabPath);
        }

        public string FormatCharacterPath(string path, string characterName) => path.Replace(CHARACTER_NAME_ID, characterName);

        private Character CreateCharacterFromInfo(CHARACTER_INFO info)
        {
            CharacterConfigData config = info.config;

            switch (config.characterType)
            {
                case Character.CharacterType.Text:
                    return new Character_Text(info.name, config);
                case Character.CharacterType.Sprite:
                case Character.CharacterType.Spritesheet:
                    return new Character_Sprite(info.name, config, info.prefab, info.rootCharacterFolder);

                default:
                    return null;
            }

        }

        private class CHARACTER_INFO
        {
            public string name = "";
            public string castingName = "";

            public string rootCharacterFolder = "";

            public CharacterConfigData config = null;
            public GameObject prefab;
        }

    }



}
