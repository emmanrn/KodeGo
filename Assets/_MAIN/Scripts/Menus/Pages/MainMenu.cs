using MAIN_GAME;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance;


    public void ClickStartNewGame()
    {
        uiChoiceMenu.Show("Start a new game?", new UIConfirmationMenu.ConfirmationButton("Yes", StartNewGame), new UIConfirmationMenu.ConfirmationButton("No", null));
    }

    private void StartNewGame()
    {
        GameSave.activeFile = new GameSave();
        VariableStore.RemoveAllVariables();
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelection");
    }

    public void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelection");
    }

}
