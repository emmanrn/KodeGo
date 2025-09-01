using DIALOGUE;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public GameObject scrollBar;
    float scrollPos = 0;
    private int currentIdx = 0;
    float[] pos;
    [SerializeField] private InputReader inputReader;

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
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1f);

        for (int i = 0; i < pos.Length; i++)
            pos[i] = distance * i;


        if (Input.GetMouseButton(0))
            scrollPos = scrollBar.GetComponent<Scrollbar>().value;
        else
            SnapToLevel(pos, distance);

        HighlightLevel(pos, distance);

    }
    private void SnapToLevel(float[] pos, float distance)
    {
        for (int i = 0; i < pos.Length; i++)
        {
            if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2))
                scrollBar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollBar.GetComponent<Scrollbar>().value, pos[i], 0.1f);

        }

    }
    private void HighlightLevel(float[] pos, float distance)
    {
        for (int i = 0; i < pos.Length; i++)
        {
            if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2))
            {
                currentIdx = i;
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);
                Color color = transform.GetChild(i).GetComponent<Image>().color;
                color.a = 1f;
                transform.GetChild(i).GetComponent<Image>().color = color;
                transform.GetChild(i).GetComponent<Button>().Select();

                for (int a = 0; a < pos.Length; a++)
                {
                    if (a != i)
                    {
                        transform.GetChild(a).localScale = Vector2.Lerp(transform.GetChild(a).localScale, new Vector2(0.8f, 0.8f), 0.1f);
                        color = transform.GetChild(a).GetComponent<Image>().color;
                        color.a = 0.5f;
                        transform.GetChild(a).GetComponent<Image>().color = color;

                    }
                }
            }

        }

    }

    private void Next()
    {
        Debug.Log("Next");
        currentIdx = Mathf.Clamp(currentIdx + 1, 0, pos.Length - 1);
        scrollPos = pos[currentIdx];
    }
    private void Previous()
    {
        Debug.Log("Prev");
        currentIdx = Mathf.Clamp(currentIdx - 1, 0, pos.Length + 1);
        scrollPos = pos[currentIdx];
    }

}
