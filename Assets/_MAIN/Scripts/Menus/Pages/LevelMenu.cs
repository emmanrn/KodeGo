using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelMenu : MonoBehaviour
{
    public GameObject scrollBar;
    float scrollPos = 0;
    private int currentIdx = 0;
    float[] pos;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private LevelDatabase_SO levelDB;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private string[] levelNames;
    [SerializeField] private TextAsset[] filesToRead;

    private Scrollbar bar;
    private bool isSmoothScrolling = false;

    private void Awake()
    {
        if (scrollBar != null) bar = scrollBar.GetComponent<Scrollbar>();
    }

    private void OnEnable()
    {
        inputReader.SetGeneral();
        inputReader.NextLevelEvent += Next;
        inputReader.PrevLevelEvent += Previous;
    }

    private void OnDisable()
    {
        inputReader.NextLevelEvent -= Next;
        inputReader.PrevLevelEvent -= Previous;
    }

    void Update()
    {
        Swipe();
    }

    public void Swipe()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        pos = new float[childCount];
        float distance = (childCount > 1) ? 1f / (childCount - 1f) : 1f;

        bool invert = false;
        if (bar == null && scrollBar != null) bar = scrollBar.GetComponent<Scrollbar>();
        if (bar != null)
        {
            // invert mapping if scrollbar direction is reversed
            invert = (bar.direction == Scrollbar.Direction.RightToLeft || bar.direction == Scrollbar.Direction.BottomToTop);
        }

        for (int i = 0; i < pos.Length; i++)
        {
            float p = distance * i;
            pos[i] = invert ? 1f - p : p;
        }

        // Always read actual scrollbar value (so programmatic moves are reflected)
        if (bar != null) scrollPos = bar.value;

        // Only auto-snap when the user is NOT dragging AND we are NOT programmatically scrolling
        if (!Input.GetMouseButton(0) && !isSmoothScrolling)
            SnapToLevel(pos, distance);

        HighlightLevel(pos, distance);

    }
    private void SnapToLevel(float[] pos, float distance)
    {
        if (bar == null) return;

        for (int i = 0; i < pos.Length; i++)
        {
            if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2))
            {
                bar.value = Mathf.Lerp(bar.value, pos[i], 0.1f);
            }
        }

    }
    private void HighlightLevel(float[] pos, float distance)
    {
        for (int i = 0; i < pos.Length; i++)
        {
            if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2))
            {
                currentIdx = i;
                ApplyImmediateHighlight(i);
            }
        }

    }

    private void Next()
    {
        currentIdx = Mathf.Clamp(currentIdx + 1, 0, pos.Length - 1);
        StopAllCoroutines();
        StartCoroutine(SmoothScrollTo(pos[currentIdx], currentIdx));
    }
    private void Previous()
    {
        currentIdx = Mathf.Clamp(currentIdx - 1, 0, pos.Length - 1);
        StopAllCoroutines();
        StartCoroutine(SmoothScrollTo(pos[currentIdx], currentIdx));
    }

    public void OnLevelButtonClicked(int levelIndex)
    {
        levelIndex -= 1;
        if (pos == null || pos.Length == 0) return;
        if (levelIndex < 0 || levelIndex >= pos.Length) return;

        bool unlocked = LevelProgressManager.runtime.ContainsKey(levelDB.levels[levelIndex].name) &&
                        LevelProgressManager.runtime[levelDB.levels[levelIndex].name].unlocked;

        if (!unlocked)
        {
            Debug.Log("Level is locked!");
            return; // prevent loading locked levels
        }

        if (levelIndex != currentIdx)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothScrollTo(pos[levelIndex], levelIndex));
        }
        else
        {
            LoadLevel(levelIndex);
        }
    }

    private IEnumerator SmoothScrollTo(float target, int targetIndex)
    {
        if (bar == null && scrollBar != null) bar = scrollBar.GetComponent<Scrollbar>();
        if (bar == null) yield break;

        // Mark that we're programmatically scrolling so SnapToLevel won't fight us
        isSmoothScrolling = true;

        // Immediately mark+show the target as selected so UI feels responsive
        currentIdx = targetIndex;
        ApplyImmediateHighlight(targetIndex);

        // Smoothly lerp scrollbar value to target
        while (Mathf.Abs(bar.value - target) > 0.001f)
        {
            bar.value = Mathf.Lerp(bar.value, target, 0.15f);
            // keep scrollPos in sync (important for HighlightLevel)
            scrollPos = bar.value;
            yield return null;
        }

        bar.value = target;
        scrollPos = target;

        // finished
        isSmoothScrolling = false;
    }

    private void ApplyImmediateHighlight(int index)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);
            Image img = t.GetComponent<Image>();
            Button btn = t.GetComponent<Button>();

            bool unlocked = LevelProgressManager.runtime.ContainsKey(levelDB.levels[i].name) &&
                            LevelProgressManager.runtime[levelDB.levels[i].name].unlocked;

            if (i == index)
            {
                t.localScale = new Vector3(1f, 1f, 1f);
                if (img != null)
                {
                    Color c = img.color;
                    c.a = unlocked ? 1f : 0.5f; // dim if locked
                    img.color = c;
                }
                if (btn != null && unlocked) btn.Select();

                if (levelNameText != null)
                {
                    string displayName = unlocked ? levelNames[i] : "???";
                    int levelNumber = i + 1; // assuming your first level is index 0

                    levelNameText.text = $"LEVEL {levelNumber}: {displayName}";
                }
            }
            else
            {
                t.localScale = new Vector3(0.8f, 0.8f, 1f);
                if (img != null)
                {
                    Color c = img.color;
                    c.a = unlocked ? 0.5f : 0.25f; // even dimmer if locked
                    img.color = c;
                }
            }

            // Disable interaction if locked
            if (btn != null) btn.interactable = unlocked;
        }
    }

    private void LoadLevel(int index)
    {
        index = index + 1;
        Debug.Log($"Loading Level {index}");
        string levelName = $"Level{index}";
        Debug.Log(filesToRead[index - 1].name);
        Transition.instance.LoadNextScene(levelName, playCutscene: true, filesToRead[index - 1]);
        // Replace this with your actual scene-loading logic:
        // SceneManager.LoadScene("Level" + index);
        // SceneManager.LoadScene($"Level {index}");
    }
    public void LoadLevel(string name)
    {
        Debug.Log($"Loading Level {name}");
        // Replace this with your actual scene-loading logic:
        // SceneManager.LoadScene("Level" + index);
    }

}
