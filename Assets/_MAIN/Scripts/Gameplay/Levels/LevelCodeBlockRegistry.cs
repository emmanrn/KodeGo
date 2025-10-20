using MAIN_GAME;
using UnityEngine;

public class LevelCodeBlockRegistry : MonoBehaviour
{
    [SerializeField] private int totalBlocks = 3;
    [SerializeField] private int totalQuiz = 1;

    // private const string BLOCKS_NAME = "Block";
    private const string QUIZ_NAME = "Quiz";
    private string levelName => GameManager.instance.LEVEL_NAME;

    void Start()
    {

        // Ensure this level has its own database
        VariableStore.CreateDatabase(levelName);
        var db = VariableStore.GetDatabase(levelName);

        // RegisterVariablesToObjects(BLOCKS_NAME, levelName, db, totalBlocks);
        RegisterVariablesToObjects(QUIZ_NAME, levelName, db, totalQuiz);

    }

    private void RegisterVariablesToObjects(string objName, string levelName, VariableStore.Database db, int count)
    {
        for (int i = 1; i <= count; i++)
        {
            string varName = $"{objName}_{i}";

            // Only create if it doesn't exist yet
            if (!db.variables.ContainsKey(varName))
            {
                VariableStore.CreateVariable($"{levelName}.{varName}", false);
            }

        }

    }
}
