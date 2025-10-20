using MAIN_GAME;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance;
    public LevelDatabase_SO levelDB;


    public void ClickStartNewGame()
    {
        uiChoiceMenu.Show("Start a new game?", new UIConfirmationMenu.ConfirmationButton("Yes", StartNewGame), new UIConfirmationMenu.ConfirmationButton("No", null));
    }

    private void StartNewGame()
    {
        GameSave.activeFile = new GameSave();
        VariableStore.RemoveAllVariables();
        LevelProgressManager.RemoveAllLevelData();
        LevelProgressManager.Initialize(levelDB);
        Game_Configuration.activeConfig.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelection");
    }

    public void LoadGame()
    {
        Game_Configuration.activeConfig.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelection");
    }

}
