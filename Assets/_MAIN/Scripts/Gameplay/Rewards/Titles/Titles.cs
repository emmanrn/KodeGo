using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using MAIN_GAME;
using TMPro;
using UnityEngine;

public class Titles : MonoBehaviour
{
    private const string STARTING_TITLE = "Rookie";
    // private string[] titles = new string[] { "Rookie", "Code Initiate", "Data Runner", "Cyber Mechanic", "Logic Enforcer", "Algorithm Architect", "Python Overload" };
    [SerializeField] private TextMeshProUGUI tmpro;


    void Start()
    {
        if (GameSave.activeFile.newGame && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "HomeScreen")
        {
            tmpro.text = STARTING_TITLE;
            return;
        }

        ApplyTitle();
    }

    private void ApplyTitle()
    {
        if (tmpro != null)
            tmpro.text = GetCurrentTitle();
    }

    private string GetCurrentTitle()
    {
        int index = 0;
        string title = "";
        foreach (LevelData data in LevelProgressManager.runtime.Values)
        {
            if (string.IsNullOrEmpty(data.title))
            {
                break;
            }

            title = data.title;
            index++;
        }

        if (index == 0)
            return STARTING_TITLE;

        return title;


    }

}
