using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Data.SqlTypes;
using System;

namespace CHARACTERS
{
    public class Character_Sprite : Character
    {
        private const string SPRITE_RENDERED_PARENT_NAME = "Renderers";
        private const string SPRITESHEET_DEFAULT_SHEETNAME = "Default";
        private const char SPRITESHEET_TEXTURE_SPRITE_DELIMITTER = '-';
        private CanvasGroup rootCG => root.GetComponent<CanvasGroup>();
        public List<CharacterSpriteLayer> layers = new();
        private string artAssetsDirectory = "";
        public override bool isVisible
        {
            get { return isRevealing || rootCG.alpha == 1; }
            set { rootCG.alpha = value ? 1 : 0; }
        }
        public Character_Sprite(string name, CharacterConfigData config, GameObject prefab, string rootAssetsFolder) : base(name, config, prefab)
        {
            rootCG.alpha = ENABLE_ON_START ? 1 : 0;
            artAssetsDirectory = rootAssetsFolder + "/Images";
            GetLayers();
            Debug.Log($"Sprite Character: {name}");
        }

        private void GetLayers()
        {
            Transform rendererRoot = animator.transform.Find(SPRITE_RENDERED_PARENT_NAME);

            if (rendererRoot == null)
                return;

            for (int i = 0; i < rendererRoot.transform.childCount; i++)
            {
                Transform child = rendererRoot.transform.GetChild(i);

                Image rendererImg = child.GetComponent<Image>();

                if (rendererImg != null)
                {
                    CharacterSpriteLayer layer = new CharacterSpriteLayer(rendererImg, i);
                    layers.Add(layer);
                    child.name = $"Layer {i}";
                }
            }
        }

        public void SetSprite(Sprite sprite, int layer = 0)
        {
            layers[layer].SetSprite(sprite);
        }

        public Sprite GetSprite(string spriteName)
        {
            if (config.characterType == CharacterType.Spritesheet)
            {
                string[] data = spriteName.Split(SPRITESHEET_TEXTURE_SPRITE_DELIMITTER);
                Sprite[] spriteArray = new Sprite[0];

                // we are checking if we have a texture and a sprite name, if its not equal to 2
                // then we know its just a sprite name.
                if (data.Length == 2)
                {
                    string textureName = data[0];
                    spriteName = data[1];

                    spriteArray = Resources.LoadAll<Sprite>($"{artAssetsDirectory}/{textureName}");

                }
                else
                {
                    spriteArray = Resources.LoadAll<Sprite>($"{artAssetsDirectory}/{SPRITESHEET_DEFAULT_SHEETNAME}");
                }
                if (spriteArray.Length == 0)
                    Debug.LogWarning($"Character '{name}' does not have an art asset called '{SPRITESHEET_DEFAULT_SHEETNAME}'");

                return Array.Find(spriteArray, sprite => sprite.name == spriteName);
            }
            else
            {
                return Resources.Load<Sprite>($"{artAssetsDirectory}/{spriteName}");
            }
        }

        public override IEnumerator ShowingOrHiding(bool show)
        {
            // basically the transsparency 1 = full opacity, 0 = transparent
            float targetAlpha = show ? 1 : 0;
            CanvasGroup self = rootCG;

            while (self.alpha != targetAlpha)
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, 3f * Time.deltaTime);
                yield return null;
            }

            CO_Revealing = null;
            CO_Hiding = null;
        }

        public override void OnReceiveCastingExpr(int layer, string expression)
        {
            Sprite sprite = GetSprite(expression);

            if (sprite == null)
            {
                Debug.LogWarning($"Sprite '{expression}' could not be found for character '{name}'");
                return;
            }

            SetSprite(sprite: sprite);
        }
    }
}