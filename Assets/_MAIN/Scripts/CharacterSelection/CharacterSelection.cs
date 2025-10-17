using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    private CharacterSkinManager skinManager => CharacterSkinManager.instance;

    public void Next()
    {
        // int nextIndex = (skinManager.skinsCount == 0) ? 0 : (skinManager.selectedSkin + 1) % skinManager.skinsCount;
        skinManager.ChangeSkin(skinManager.selectedSkin + 1);
    }

    public void Previous()
    {
        // int previousIndex = skinManager.selectedSkin - 1;
        // if (previousIndex < 0)
        //     previousIndex += skinManager.skinsCount;

        skinManager.ChangeSkin(skinManager.selectedSkin - 1);
    }

    public void Apply() => skinManager.ApplySkin();


}
