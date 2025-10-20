using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        Game_Configuration.activeConfig.Save();
        SceneManager.LoadScene(sceneName);
    }

    public void NextLevel()
    {
        Game_Configuration.activeConfig.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Gameplay 2");
    }

    public void Quit()
    {
        Game_Configuration.activeConfig.Save();
        Application.Quit();
    }
}
