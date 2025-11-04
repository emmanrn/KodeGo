using System.Collections.Generic;
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
    private List<int> UnlockedSkinIndices =>
    config.skins
        .Select((s, i) => new { s, i })
        .Where(x => IsSkinUnlocked(x.s.name))
        .Select(x => x.i)
        .ToList();


    void Start()
    {
        instance = this;

        if (GameSave.activeFile.newGame)
        {
            ChangeSkin(0);
            return;
        }


        if (VariableStore.TryGetValue(varKey, out object savedSkin))
        {
            int index = (int)savedSkin;
            Debug.Log(index);
            if (index >= 0 && index < config.skins.Length)
                selectedSkin = index;
        }

        // Apply the initial skin
        ApplySkinColor(config.skins[selectedSkin].color);

    }

    public void ChangeSkin(int direction = 1)
    {
        if (config == null || config.skins == null || config.skins.Length == 0)
            return;

        var unlocked = UnlockedSkinIndices;
        if (unlocked.Count == 0)
            return;

        // Find the current position among unlocked skins
        int currentUnlockedIndex = unlocked.IndexOf(selectedSkin);
        if (currentUnlockedIndex == -1)
            currentUnlockedIndex = 0;

        // If direction = 0, just apply current
        if (direction != 0)
            currentUnlockedIndex = ((currentUnlockedIndex + direction) % unlocked.Count + unlocked.Count) % unlocked.Count;

        selectedSkin = unlocked[currentUnlockedIndex];
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
        if (config.skins == null)
            return 0;

        int count = 0;
        foreach (var skin in config.skins)
        {
            if (IsSkinUnlocked(skin.name))
                count++;
        }

        return count;
    }

    public bool IsSkinUnlocked(string skinName)
    {
        if (config == null || config.skins == null)
            return false;

        // Skin 0 is always unlocked
        if (config.skins[0].name == skinName)
            return true;

        // Check all levels — since one level unlocks exactly one skin
        foreach (var levelData in LevelProgressManager.runtime.Values)
        {
            if (!string.IsNullOrEmpty(levelData.skinUnlocked) &&
                levelData.skinUnlocked == skinName)
            {
                return true;
            }
        }

        return false;
    }


    public void ApplySkin()
    {
        VariableStore.TrySetValue(varKey, selectedSkin);
    }
}
