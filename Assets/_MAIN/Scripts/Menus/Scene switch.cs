using MAIN_GAME;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        Game_Configuration.activeConfig.Save();
        GameSave.activeFile.Save();

        AudioManager.instance.StopAllTracks();
        AudioManager.instance.PlayTrack(FilePaths.GetPathToResource(FilePaths.resources_music, "HSBG"), loop: true, startingVolume: 0f, volumeCap: 0.7f);
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
