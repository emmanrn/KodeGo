using System.Linq;
using MAIN_GAME;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSkinManager : MonoBehaviour
{
    public static CharacterSkinManager instance { get; private set; }


    public const string DB_NAME = "Player";
    public const string VAR_NAME = "SelectedSkin";
    public string varKey => $"{DB_NAME}.{VAR_NAME}";
    [SerializeField] private CharacterSkinConfig config;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Image img;
    public bool changePlayerSkin = false;
    public int skinsCount => GetSkinCount();
    public int selectedSkin { get; private set; } = 0;


    void Start()
    {
        instance = this;

        if (GameSave.activeFile.newGame)
        {
            ChangeSkin(selectedSkin);
            return;
        }


        if (VariableStore.TryGetValue(varKey, out object savedSkin))
        {
            int index = (int)savedSkin;
            Debug.Log(index);
            if (index >= 0 && index < skinsCount)
                selectedSkin = index;
        }

        // Apply the initial skin
        ChangeSkin(selectedSkin);

    }

    public void ChangeSkin(int index)
    {
        selectedSkin = (skinsCount == 0)
      ? 0
      : (index < 0 ? index + skinsCount : (index >= skinsCount ? index - skinsCount : index));


        if (selectedSkin == 0)
        {
            ApplySkinColor(Color.white);
            return;
        }

        CharacterSkin skin = config.skins[selectedSkin];
        ApplySkinColor(skin.color);
    }

    private void ApplySkinColor(Color color)
    {
        if (sr != null)
            sr.color = color;
        if (img != null)
            img.color = color;
    }
    public int GetSkinCount()
    {
        if (config.skins == null || LevelProgressManager.runtime.Values == null)
            return 0;

        int count = 1;

        foreach (var skin in config.skins)
        {
            // Check if any runtime value matches this skin's name
            if (LevelProgressManager.runtime.Values.Any(r => r.skinUnlocked == skin.name))
                count++;
        }

        return count;
    }


    public void ChangeSkinUI(int index)
    {
        if (config.skins == null || skinsCount == 0 || img == null)
            return;

        if (index < 0 || index >= skinsCount)
            return;

        selectedSkin = index; // update the stored index
        CharacterSkin skin = config.skins[index];
        img.color = skin.color;

    }

    public void ChangeSkinPlayer(int index)
    {
        if (config.skins == null || skinsCount == 0 || sr == null)
            return;

        if (index < 0 || index >= skinsCount)
            return;

        selectedSkin = index; // update the stored index
        CharacterSkin skin = config.skins[index];
        sr.color = skin.color;

    }

    public void ApplySkin()
    {
        VariableStore.TrySetValue(varKey, selectedSkin);
    }
}
