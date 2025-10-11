using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private GameObject[] skins;
    public int selectedSkin = 0;

    public void Next()
    {
        skins[selectedSkin].SetActive(false);
        selectedSkin = (selectedSkin + 1) % skins.Length;
        skins[selectedSkin].SetActive(true);

    }

    public void Previous()
    {
        skins[selectedSkin].SetActive(false);
        selectedSkin--;

        if (selectedSkin < 0)
            selectedSkin += skins.Length;

        skins[selectedSkin].SetActive(true);

    }


}
