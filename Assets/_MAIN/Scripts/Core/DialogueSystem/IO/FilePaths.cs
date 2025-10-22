public class FilePaths
{
    private const string HOME_DIRECTORY_SYMBOL = "~/";

    public static readonly string root = $"{runtimePath}/gameData/";

    //Runtime Paths
    public static readonly string gameSaves = $"{runtimePath}Save Files/";

    // Resources Paths
    // Graphics Path
    public static readonly string resources_graphics = "Graphics/";

    //Audio Paths
    public static readonly string resources_audio = "Audio/";
    public static readonly string resources_sfx = $"{resources_audio}SFX/";
    public static readonly string resources_music = $"{resources_audio}Music/";
    public static readonly string resources_ambience = $"{resources_audio}Ambience/";
    public static readonly string resources_voices = $"{resources_audio}Voices/";

    // Dialogue Files Paths
    public static readonly string resources_dialogueFiles = $"Dialogue Files/";
    public static readonly string resources_testDialogueFiles = $"{resources_dialogueFiles}Testing/";

    public static string GetPathToResource(string defaultPath, string resourceName)
    {
        if (resourceName.StartsWith(HOME_DIRECTORY_SYMBOL))
            return resourceName.Substring(HOME_DIRECTORY_SYMBOL.Length);

        return defaultPath + resourceName;
    }

    public static string runtimePath
    {
        get
        {
#if UNITY_EDITOR
            return "Assets/appdata/";
#else
            return Application.persistentDataPath + "/appdata/";
#endif
        }
    }

}
