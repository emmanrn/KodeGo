using TERMINAL;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager instance { get; private set; }

    [SerializeField] private QuestionsProvider provider;
    void Awake()
    {
        instance = this;

        provider.Initialize();
    }


    public CodeTerminalConfig GetRandomQuestion(Terminal.TerminalType terminalType)
    {
        return provider.GetRandomQuestions(terminalType);
    }
}
