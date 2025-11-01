using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    private CharacterSkinManager skinManager => CharacterSkinManager.instance;

    public void Next()
    {
        skinManager.ChangeSkin(skinManager.selectedSkin + 1);
    }

    public void Previous()
    {
        skinManager.ChangeSkin(skinManager.selectedSkin - 1);
    }

    public void Apply() => skinManager.ApplySkin();


}
