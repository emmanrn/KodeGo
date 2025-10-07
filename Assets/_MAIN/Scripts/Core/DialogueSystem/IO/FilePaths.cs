using UnityEngine;

public class FilePaths
{
    private const string HOME_DIRECTORY_SYMBOL = "~/";

    public static readonly string root = $"{Application.dataPath}/gameData/";

    // Resources Paths
    // Graphics Path
    public static readonly string resources_graphics = "Graphics/";

    //Audio Paths
    public static readonly string resources_audio = "Audio/";

    // Dialogue Files Paths
    public static readonly string resources_dialogueFiles = $"Dialogue Files/";
    public static readonly string resources_testDialogueFiles = $"{resources_dialogueFiles}Testing/";

    public static string GetPathToResource(string defaultPath, string resourceName)
    {
        if (resourceName.StartsWith(HOME_DIRECTORY_SYMBOL))
            return resourceName.Substring(HOME_DIRECTORY_SYMBOL.Length);

        return defaultPath + resourceName;
    }

}
