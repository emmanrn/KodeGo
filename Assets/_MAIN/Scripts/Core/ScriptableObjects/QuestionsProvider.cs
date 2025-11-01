using System.Collections;
using System.Collections.Generic;
using TERMINAL;
using UnityEngine;

[CreateAssetMenu(fileName = "Question Provider", menuName = "Question Provider")]
public class QuestionsProvider : ScriptableObject
{

    public List<FinalTerminalConfig> finalDatabase;
    public List<DebugTerminalConfig> debugDatabase;
    public List<PracticeTermConfig> practiceDatabase;
    private Dictionary<Terminal.TerminalType, List<CodeTerminalConfig>> questions = new Dictionary<Terminal.TerminalType, List<CodeTerminalConfig>>();

    public void Initialize()
    {
        questions[Terminal.TerminalType.FINAL] = new List<CodeTerminalConfig>(finalDatabase);
        questions[Terminal.TerminalType.DEBUG] = new List<CodeTerminalConfig>(debugDatabase);
        questions[Terminal.TerminalType.PRACTICE] = new List<CodeTerminalConfig>(practiceDatabase);
    }

    public CodeTerminalConfig GetRandomQuestions(Terminal.TerminalType terminalType)
    {
        if (!questions.ContainsKey(terminalType))
        {
            Debug.LogWarning($"Could not find terminal type of '{terminalType}'");
            return null;
        }

        var list = questions[terminalType];

        int index = Random.Range(0, list.Count);
        CodeTerminalConfig config = list[index];
        list.RemoveAt(index);
        return config;
    }
}
