using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredefinedFuncs
{
    public RectTransform playerRb;
    private float movementSpeed = 50f;
    private Vector3 targetPos;
    private float distance = 50f;
    public bool isMoving = false;

    void Start()
    {
        targetPos = new Vector3(playerRb.localPosition.x + distance, playerRb.localPosition.y);
    }

    public IEnumerator PlayerMoveRight()
    {
        if (isMoving)
            yield break;


        if (!isMoving)
        {
            while (playerRb.localPosition != targetPos)
            {
                isMoving = true;
                playerRb.localPosition = Vector3.MoveTowards(playerRb.localPosition, targetPos, movementSpeed * Time.deltaTime);

                yield return null;

            }
            targetPos = new Vector3(playerRb.localPosition.x + distance, playerRb.localPosition.y);
            isMoving = false;

        }
    }
}
