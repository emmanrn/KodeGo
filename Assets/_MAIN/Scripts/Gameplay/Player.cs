using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform playerRb;
    [SerializeField] Rigidbody2D rb;
    private float movementSpeed = 5f;
    private float jumpForce = 10f;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float castDistance = 0.5f;
    private GameStateManager gameState => GameStateManager.instance;
    private bool isJumping { get; set; }
    private float distance = 5f;
    public bool isMoving = false;
    [SerializeField] private LayerMask groundLayer;
    public bool isGrounded { get { return Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer); } }



    public IEnumerator PlayerMoveRight()
    {
        if (!isGrounded)
            yield break;

        isMoving = true;
        Vector2 targetPos = rb.position + Vector2.right * distance;
        while (Vector2.Distance(rb.position, targetPos) > 0.01f)
        {
            if (gameState.isPaused)
                yield return new WaitUntil(() => !gameState.isPaused);
            else if (gameState.isRunningDialogue)
            {
                isMoving = false;
                yield break;
            }

            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, movementSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }
        rb.position = targetPos;
        isMoving = false;
    }
    public IEnumerator PlayerMoveLeft()
    {
        if (!isGrounded)
            yield break;

        isMoving = true;
        Vector2 targetPos = rb.position + Vector2.left * distance;
        while (Vector2.Distance(rb.position, targetPos) > 0.01f)
        {
            if (gameState.isPaused)
                yield return new WaitUntil(() => !gameState.isPaused);
            else if (gameState.isRunningDialogue)
            {
                isMoving = false;
                yield break;
            }

            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, movementSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }
        rb.position = targetPos;
        isMoving = false;
    }

    public IEnumerator Jump()
    {
        if (isGrounded)
            yield break;

        isMoving = true;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        while (!isGrounded)
        {
            if (gameState.isPaused)
                yield return new WaitUntil(() => !gameState.isPaused);
            else if (gameState.isRunningDialogue)
            {
                isMoving = false;
                yield break;
            }

            yield return null;
        }
        isMoving = false;

    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }


}
