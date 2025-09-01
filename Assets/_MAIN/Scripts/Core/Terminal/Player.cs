using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public RectTransform playerRb;
    private float movementSpeed = 100f;
    private Vector3 targetPos;
    private float distance = 50f;
    public bool isMoving = false;

    void Start()
    {
        targetPos = new Vector3(playerRb.localPosition.x, playerRb.localPosition.y);
    }


    public IEnumerator PlayerMoveRight()
    {
        if (isMoving)
            yield break;


        if (!isMoving)
        {
            targetPos = new Vector3(playerRb.localPosition.x + distance, playerRb.localPosition.y);
            while (playerRb.localPosition != targetPos)
            {
                isMoving = true;
                playerRb.localPosition = Vector3.MoveTowards(playerRb.localPosition, targetPos, movementSpeed * Time.deltaTime);

                yield return null;

            }
            isMoving = false;

        }


    }
    public IEnumerator PlayerMoveLeft()
    {
        if (isMoving)
            yield break;


        if (!isMoving)
        {
            targetPos = new Vector3(playerRb.localPosition.x - distance, playerRb.localPosition.y);
            while (playerRb.localPosition != targetPos)
            {
                isMoving = true;
                playerRb.localPosition = Vector3.MoveTowards(playerRb.localPosition, targetPos, movementSpeed * Time.deltaTime);

                yield return null;

            }
            isMoving = false;

        }
    }


}
