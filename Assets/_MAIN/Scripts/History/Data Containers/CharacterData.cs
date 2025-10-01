using System.Collections.Generic;
using CHARACTERS;
using UnityEngine;

namespace HISTORY
{
    // maybe same with this because i dont think we're gonna need to save the character here
    // character in question is the character that is speaking the dialogue
    // BUT JUST IN CASE, ill try to implement it anyways
    [System.Serializable]
    public class CharacterData
    {
        public string characterName;
        public string displayName;
        public bool enabled;
        public string dataJSON;
        public string animJSON;
        public CharacterConfigCache characterConfig;

        [System.Serializable]
        public class CharacterConfigCache
        {
            public string name;
            public string alias;

            public Character.CharacterType characterType;

            public Color nameColor;
            public Color dialogueColor;

            public float nameFontScale = 1f;
            public float dialogueFontScale = 1f;


            public CharacterConfigCache(CharacterConfigData reference)
            {
                name = reference.name;
                alias = reference.alias;
                characterType = reference.characterType;

                nameColor = reference.nameColor;
                dialogueColor = reference.dialogueColor;

                nameFontScale = reference.nameFontScale;
                dialogueFontScale = reference.dialogueFontScale;

            }

        }

        public static List<CharacterData> Capture()
        {
            List<CharacterData> characters = new List<CharacterData>();

            foreach (var character in CharacterManager.instance.allCharacters)
            {
                // we dont record hidden characters, because they're not on the screen at that time
                // and we also dont wanna waste file space for their details
                if (!character.isVisible)
                    continue;

                CharacterData entry = new CharacterData();
                entry.characterName = character.name;
                entry.displayName = character.displayName;
                entry.enabled = character.isVisible;
                entry.characterConfig = new CharacterConfigCache(character.config);
                entry.animJSON = GetAnimationData(character);

                switch (character.config.characterType)
                {
                    case Character.CharacterType.Sprite:
                    case Character.CharacterType.Spritesheet:
                        SpriteData sData = new SpriteData();
                        sData.layers = new List<SpriteData.LayerData>();

                        Character_Sprite sc = character as Character_Sprite;
                        foreach (var layer in sc.layers)
                        {
                            var layerData = new SpriteData.LayerData();
                            layerData.color = layer.renderer.color;
                            layerData.spriteName = layer.renderer.name;
                            sData.layers.Add(layerData);
                        }

                        entry.dataJSON = JsonUtility.ToJson(sData);
                        break;
                }
                characters.Add(entry);
            }
            return characters;
        }

        public static void Apply(List<CharacterData> data)
        {
            List<string> cache = new List<string>();

            foreach (CharacterData characterData in data)
            {
                Character character = CharacterManager.instance.GetCharacter(characterData.characterName, createIfDoesnNotExist: true);

                character.displayName = characterData.displayName;
                character.isVisible = characterData.enabled;

                AnimationData animationData = JsonUtility.FromJson<AnimationData>(characterData.animJSON);
                ApplyAnimationData(character, animationData);

                switch (character.config.characterType)
                {
                    case Character.CharacterType.Sprite:
                    case Character.CharacterType.Spritesheet:
                        SpriteData sData = JsonUtility.FromJson<SpriteData>(characterData.dataJSON);
                        Character_Sprite sc = character as Character_Sprite;

                        for (int i = 0; i < sData.layers.Count; i++)
                        {
                            var layer = sData.layers[i];
                            if (sc.layers[i].renderer.sprite != null && sc.layers[i].renderer.sprite.name != layer.spriteName)
                            {
                                Sprite sprite = sc.GetSprite(layer.spriteName);
                                if (sprite != null)
                                    sc.SetSprite(sprite, i);
                                else
                                    Debug.LogWarning($"History state could not load sprite '{layer.spriteName}");
                            }
                        }
                        break;
                }
                cache.Add(character.name);
            }
            foreach (Character character in CharacterManager.instance.allCharacters)
            {
                if (!cache.Contains(character.name))
                    character.isVisible = false;
            }
        }

        private static string GetAnimationData(Character character)
        {
            Animator animator = character.animator;
            AnimationData data = new AnimationData();

            foreach (var param in animator.parameters)
            {
                // we skip the trigger type of animation param because we dont need to save the trigger
                if (param.type == AnimatorControllerParameterType.Trigger)
                    continue;

                AnimationData.AnimationParameter pData = new AnimationData.AnimationParameter { name = param.name };

                switch (param.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        pData.type = "Bool";
                        pData.value = animator.GetBool(param.name).ToString();
                        break;
                    case AnimatorControllerParameterType.Float:
                        pData.type = "Float";
                        pData.value = animator.GetFloat(param.name).ToString();
                        break;
                    case AnimatorControllerParameterType.Int:
                        pData.type = "Int";
                        pData.value = animator.GetInteger(param.name).ToString();
                        break;

                }
                data.parameters.Add(pData);
            }

            return JsonUtility.ToJson(data);

        }

        private static void ApplyAnimationData(Character character, AnimationData data)
        {
            Animator animator = character.animator;
            foreach (var param in data.parameters)
            {
                switch (param.type)
                {
                    case "Bool":
                        animator.SetBool(param.name, bool.Parse(param.value));
                        break;
                    case "Float":
                        animator.SetFloat(param.name, float.Parse(param.value));
                        break;
                    case "Int":
                        animator.SetInteger(param.name, int.Parse(param.value));
                        break;
                }
            }

            // not yet needed since again we're not doing animations for now
            // but if ever we do, then after we apply the animation
            // we need to refresh the animation and apply the animation again
            // animator.SetTrigger(Character.ANIMATION_REFRESH_TRIGGER);
        }

        // probably not gonna use animations since we are just changing expressions and all,
        // but just in case if we want to have the character icons to move then here it is
        [System.Serializable]
        public class AnimationData
        {
            public List<AnimationParameter> parameters = new List<AnimationParameter>();

            [System.Serializable]
            public class AnimationParameter
            {
                public string name;
                public string type;
                public string value;
            }
        }
        [System.Serializable]
        public class SpriteData
        {
            public List<LayerData> layers;
            [System.Serializable]
            public class LayerData
            {
                public string spriteName;
                public Color color;
            }
        }
    }

}
