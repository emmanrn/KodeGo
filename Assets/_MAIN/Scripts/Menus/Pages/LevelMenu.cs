using System.Collections;
using DIALOGUE;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public GameObject scrollBar;
    float scrollPos = 0;
    private int currentIdx = 0;
    float[] pos;
    [SerializeField] private InputReader inputReader;

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

                Transform current = transform.GetChild(i);
                current.localScale = Vector3.Lerp(current.localScale, new Vector3(1f, 1f, 1f), 0.1f);
                Image imgCurrent = current.GetComponent<Image>();
                if (imgCurrent != null)
                {
                    Color c = imgCurrent.color;
                    c.a = 1f;
                    imgCurrent.color = c;
                }
                Button btn = current.GetComponent<Button>();
                if (btn != null) btn.Select();

                // Dim others
                for (int a = 0; a < pos.Length; a++)
                {
                    if (a != i)
                    {
                        Transform other = transform.GetChild(a);
                        other.localScale = Vector3.Lerp(other.localScale, new Vector3(0.8f, 0.8f, 1f), 0.1f);
                        Image imgOther = other.GetComponent<Image>();
                        if (imgOther != null)
                        {
                            Color c = imgOther.color;
                            c.a = 0.5f;
                            imgOther.color = c;
                        }
                    }
                }
            }
        }

    }

    private void Next()
    {
        Debug.Log("Next");
        currentIdx = Mathf.Clamp(currentIdx + 1, 0, pos.Length - 1);
        StopAllCoroutines();
        StartCoroutine(SmoothScrollTo(pos[currentIdx], currentIdx));
    }
    private void Previous()
    {
        Debug.Log("Prev");
        currentIdx = Mathf.Clamp(currentIdx - 1, 0, pos.Length - 1);
        StopAllCoroutines();
        StartCoroutine(SmoothScrollTo(pos[currentIdx], currentIdx));
    }

    public void OnLevelButtonClicked(int levelIndex)
    {
        levelIndex -= 1;
        if (pos == null || pos.Length == 0) return;
        if (levelIndex < 0 || levelIndex >= pos.Length) return;

        // If the clicked level is not highlighted yet -> smooth-scroll to it (and visually highlight)
        if (levelIndex != currentIdx)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothScrollTo(pos[levelIndex], levelIndex));
        }
        else
        {
            // It's already the highlighted one → load scene
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

            if (i == index)
            {
                t.localScale = new Vector3(1f, 1f, 1f);
                if (img != null)
                {
                    Color c = img.color;
                    c.a = 1f;
                    img.color = c;
                }
                Button b = t.GetComponent<Button>();
                if (b != null) b.Select();
            }
            else
            {
                t.localScale = new Vector3(0.8f, 0.8f, 1f);
                if (img != null)
                {
                    Color c = img.color;
                    c.a = 0.5f;
                    img.color = c;
                }
            }
        }
    }

    private void LoadLevel(int index)
    {
        Debug.Log($"Loading Level {index}");
        // Replace this with your actual scene-loading logic:
        // SceneManager.LoadScene("Level" + index);
        SceneManager.LoadScene("Gameplay 2");
    }
    public void LoadLevel(string name)
    {
        Debug.Log($"Loading Level {name}");
        // Replace this with your actual scene-loading logic:
        // SceneManager.LoadScene("Level" + index);
    }

}
