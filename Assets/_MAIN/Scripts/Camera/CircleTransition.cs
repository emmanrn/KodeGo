using System.Collections;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            OpenCircle();
        else if (Input.GetKeyDown(KeyCode.Backspace))
            CloseCircle();
    }


    public IEnumerator OpenCircle()
    {
        DrawBlackScreen();
        yield return StartCoroutine(Transition(1, 0, 1.5f));
    }

    public IEnumerator CloseCircle()
    {
        DrawBlackScreen();
        yield return StartCoroutine(Transition(1, 1.5f, 0));
    }

    private void DrawBlackScreen()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;


        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;
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

        // if (canvasWidth > canvasHeight)
        // {
        //     squareValue = canvasWidth;
        //     playerCanvasPos.y += (canvasWidth - canvasHeight) * 0.5f;
        // }
        // else
        // {
        //     squareValue = canvasHeight;
        //     playerCanvasPos.x += (canvasHeight - canvasHeight) * 0.5f;
        // }

        playerCanvasPos.x = (playerCanvasPos.x / canvasRect.rect.width) + 0.5f;
        playerCanvasPos.y = (playerCanvasPos.y / canvasRect.rect.height) + 0.5f;

        Material mat = blackScreen.material;
        mat.SetFloat(CENTER_X, playerCanvasPos.x);
        mat.SetFloat(CENTER_Y, playerCanvasPos.y);

        float squareValue = Mathf.Max(canvasRect.rect.width, canvasRect.rect.height);
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
