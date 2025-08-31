using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextArchitect
{
    private TextMeshProUGUI tmproUI;
    private TextMeshPro tmproWorld;
    public TMP_Text tmpro => tmproUI != null ? tmproUI : tmproWorld;


    public string currentTxt => tmpro.text;
    public string targetTxt { get; private set; } = "";
    public string preTxt { get; private set; } = "";
    private int preTxtLen = 0;

    public string fullTargetTxt => preTxt + targetTxt;

    // how we want the text to be put in the the dialogue
    public enum BuildMethod { INSTANT, TYPEWRITER, FADE }
    public BuildMethod buildMethod = BuildMethod.TYPEWRITER;

    public Color txtColor { get { return tmpro.color; } set { tmpro.color = value; } }

    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    private const float baseSpeed = 1;

    //configurable by user
    private float speedMultiplier = 1;

    //basically how many characters are fading in
    public int charPerCycle { get { return speed <= 2f ? charMult : speed <= 2.5f ? charMult * 2 : charMult * 3; } }

    private int charMult = 1;
    public bool hurryUp = false;


    // Constructors init here
    public TextArchitect(TextMeshProUGUI tmproUI)
    {
        this.tmproUI = tmproUI;
    }
    public TextArchitect(TextMeshPro tmproWorld)
    {
        this.tmproWorld = tmproWorld;
    }

    public Coroutine Build(string text)
    {
        preTxt = "";
        targetTxt = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }
    // Append text to the current text in the tmpro.text
    public Coroutine Append(string text)
    {
        preTxt = tmpro.text;
        targetTxt = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    private Coroutine buildProcess = null;
    public bool isBuilding => buildProcess != null;

    // Basically just stop the coroutine buildProcess of that text
    public void Stop()
    {
        if (!isBuilding)
            return;

        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    IEnumerator Building()
    {
        Prepare();

        switch (buildMethod)
        {
            case BuildMethod.TYPEWRITER:
                yield return BuildTypewriter();
                break;
            case BuildMethod.FADE:
                yield return BuildFade();
                break;
        }

        OnComplete();
    }

    private void OnComplete()
    {
        buildProcess = null;
        hurryUp = false;
    }

    public void ForceComplete()
    {
        switch (buildMethod)
        {
            case BuildMethod.TYPEWRITER:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
            case BuildMethod.FADE:
                break;
        }
        Stop();
        OnComplete();
    }

    // this one here is so that we know what the build method of the dialogue will be
    // and prepare the system on how to build it depending on the build method
    private void Prepare()
    {
        switch (buildMethod)
        {
            case BuildMethod.INSTANT:
                PrepareInstant();
                break;
            case BuildMethod.TYPEWRITER:
                PrepareTypewriter();
                break;
            case BuildMethod.FADE:
                PrepareFade();
                break;
        }

    }

    private void PrepareInstant()
    {
        // forcing the text color to apply the color to itself again
        tmpro.color = tmpro.color;
        tmpro.text = fullTargetTxt;

        // basically any changes to the text will get applied at this point
        tmpro.ForceMeshUpdate();
        // making sure every character is visible on screen
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }
    private void PrepareTypewriter()
    {
        tmpro.color = tmpro.color;
        tmpro.maxVisibleCharacters = 0;
        tmpro.text = preTxt;

        // this is making sure that if we have a preText it is making sure that the preText is visible
        if (preTxt != "")
        {
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }

        tmpro.text += targetTxt;
        tmpro.ForceMeshUpdate();
    }
    private void PrepareFade()
    {

    }

    private IEnumerator BuildTypewriter()
    {
        while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += hurryUp ? charPerCycle * 5 : charPerCycle;
            yield return new WaitForSeconds(0.015f / speed);
        }
    }
    private IEnumerator BuildFade()
    {
        yield return null;
    }


}
