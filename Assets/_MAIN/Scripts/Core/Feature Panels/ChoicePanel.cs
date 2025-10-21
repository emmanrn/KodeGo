using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// this script is for showing the choice panel that shows a prompt for a user 
// to make a choice from provided options
public class ChoicePanel : MonoBehaviour
{
    public static ChoicePanel instance { get; private set; }

    private const float BUTTON_MIN_WIDTH = 50f;
    private const float BUTTON_MAX_WIDTH = 1000f;
    private const float BUTTON_WIDTH_PADDING = 100f;

    private const float BUTTON_HEIGHT_PER_LINE = 50f;
    private const float BUTTON_HEIGHT_PADDING = 20f;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private GameObject choiceBtnPrefab;
    [SerializeField] private VerticalLayoutGroup btnLayoutGroup;

    private CanvasGroupController cg = null;
    private List<ChoiceButton> buttons = new List<ChoiceButton>();
    public ChoicePanelDecision lastDecision { get; private set; }

    public bool isWaitingOnUserChoice { get; private set; } = false;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        cg = new CanvasGroupController(this, canvasGroup);

        cg.alpha = 0;
        cg.SetInteractableState(false);
    }

    public void Show(string question, string[] choices)
    {
        lastDecision = new ChoicePanelDecision(question, choices);

        isWaitingOnUserChoice = true;

        cg.Show();
        cg.SetInteractableState(active: true);

        titleTxt.text = question;
        StartCoroutine(GenerateChoices(choices));
    }

    private IEnumerator GenerateChoices(string[] choices)
    {
        // we need to keep track of this so that all of the choices buttons all unified in sizes
        // meaning all will have the same width.
        // to do that we keep track on which of the choices buttons has the largest width
        float maxWidth = 0;

        for (int i = 0; i < choices.Length; i++)
        {
            ChoiceButton choiceButton;

            // if there is already a button ready in the scene
            if (i < buttons.Count)
            {
                choiceButton = buttons[i];
            }
            // else make a new or add a new button to the scene and add it to the list
            else
            {
                GameObject newButtonObj = Instantiate(choiceBtnPrefab, btnLayoutGroup.transform);
                newButtonObj.SetActive(true);

                Button newButton = newButtonObj.GetComponent<Button>();
                TextMeshProUGUI newTitle = newButton.GetComponentInChildren<TextMeshProUGUI>();
                LayoutElement newLayout = newButton.GetComponent<LayoutElement>();

                choiceButton = new ChoiceButton { button = newButton, layout = newLayout, title = newTitle };

                buttons.Add(choiceButton);
            }

            choiceButton.button.onClick.RemoveAllListeners();
            int buttonIndex = i;
            choiceButton.button.onClick.AddListener(() => AcceptAnswer(buttonIndex));
            choiceButton.title.text = choices[i];

            float buttonWidth = Mathf.Clamp(BUTTON_WIDTH_PADDING + choiceButton.title.preferredWidth, BUTTON_MIN_WIDTH, BUTTON_MAX_WIDTH);
            maxWidth = Mathf.Max(maxWidth, buttonWidth);
        }

        foreach (var button in buttons)
        {
            button.layout.preferredWidth = maxWidth;
        }

        // only enable the buttons that are needed
        for (int i = 0; i < buttons.Count; i++)
        {
            bool show = i < choices.Length;

            buttons[i].button.gameObject.SetActive(show);


        }

        yield return new WaitForEndOfFrame();

        foreach (var button in buttons)
        {
            int lines = button.title.textInfo.lineCount;
            button.layout.preferredHeight = BUTTON_HEIGHT_PADDING + (BUTTON_HEIGHT_PER_LINE * lines);
        }
    }

    public void Hide()
    {
        cg.Hide();
        cg.SetInteractableState(false);
    }

    private void AcceptAnswer(int index)
    {
        // if we have an invalid decision then dont accept that answer
        if (index < 0 || index > lastDecision.choices.Length - 1)
            return;

        lastDecision.answerIndex = index;
        isWaitingOnUserChoice = false;
        Hide();
    }

    // this class is basically a data containing the decisions data
    // this is where we cache the quesions, answer indexes that will represent the button that the player clicks
    public class ChoicePanelDecision
    {
        public string question = string.Empty;
        public int answerIndex = -1;
        public string[] choices = new string[0];

        public ChoicePanelDecision(string question, string[] choices)
        {
            this.question = question;
            this.choices = choices;
            answerIndex = -1;
        }

    }

    private struct ChoiceButton
    {
        public Button button;
        public TextMeshProUGUI title;
        public LayoutElement layout;
    }
}
