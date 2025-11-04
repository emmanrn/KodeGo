using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class CircleTransition : MonoBehaviour
{
    private Canvas canvas;
    private Image blackScreen;
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;
    private Vector2 playerCanvasPos;

    private static readonly int RADIUS = Shader.PropertyToID("_Radius");
    private static readonly int CENTER_X = Shader.PropertyToID("_CenterX");
    private static readonly int CENTER_Y = Shader.PropertyToID("_CenterY");

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        blackScreen = GetComponentInChildren<Image>();
        blackScreen.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            OpenCircle();
        else if (Input.GetKeyDown(KeyCode.Backspace))
            CloseCircle();
    }


    public IEnumerator OpenCircle(bool center = false)
    {
        DrawBlackScreen(center);
        yield return StartCoroutine(Transition(1, 0, 1.5f));
        blackScreen.gameObject.SetActive(false);
    }

    public IEnumerator CloseCircle(bool center = false)
    {
        blackScreen.gameObject.SetActive(true);
        DrawBlackScreen(center);
        yield return StartCoroutine(Transition(1, 1.5f, 0));
    }

    private void DrawBlackScreen(bool center)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;


        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;
        float squareValue;
        Material mat;

        if (center)
        {
            if (canvasWidth > canvasHeight)
            {
                squareValue = canvasWidth;
            }
            else
            {
                squareValue = canvasHeight;
            }
            mat = blackScreen.material;
            mat.SetFloat(CENTER_X, 0.5f);
            mat.SetFloat(CENTER_Y, 0.5f);

            blackScreen.rectTransform.sizeDelta = new Vector2(squareValue, squareValue);
            return;
        }
        // Vector3 playerPos = cam.ScreenToWorldPoint(player.position);

        // playerCanvasPos = new Vector2
        // {
        //     x = (playerPos.x / screenWidth) * canvasWidth,
        //     y = (playerPos.y / screenHeight) * canvasHeight
        // };

        Vector2 screenPos = cam.WorldToScreenPoint(player.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos,
            cam,
            out playerCanvasPos
        );



        playerCanvasPos.x = (playerCanvasPos.x / canvasRect.rect.width) + 0.5f;
        playerCanvasPos.y = (playerCanvasPos.y / canvasRect.rect.height) + 0.5f;

        mat = blackScreen.material;
        mat.SetFloat(CENTER_X, playerCanvasPos.x);
        mat.SetFloat(CENTER_Y, playerCanvasPos.y);

        squareValue = Mathf.Max(canvasRect.rect.width, canvasRect.rect.height);
        blackScreen.rectTransform.sizeDelta = new Vector2(squareValue, squareValue);


    }

    private IEnumerator Transition(float duration, float beginRadius, float endRadius)
    {

        var mat = blackScreen.material;
        var time = 0f;
        while (time <= duration)
        {
            time += Time.deltaTime;
            var t = time / duration;
            var radius = Mathf.Lerp(beginRadius, endRadius, t);

            mat.SetFloat(RADIUS, radius);

            yield return null;
        }

    }
}
