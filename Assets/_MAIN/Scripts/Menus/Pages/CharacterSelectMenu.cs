using MAIN_GAME;
using UnityEngine;
using System.IO;

public class CharacterSelectMenu : MenuPage
{
    [SerializeField] private CharacterSelection selection;

    public void Next() => selection.Next();
    public void Previous() => selection.Previous();

    public void Apply() => selection.Apply();

}
