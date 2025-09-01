using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    public float jumpPower = 5f;
    [SerializeField] private InputReader input;

    private Vector2 moveDirection;
    private bool isJumping;

    void OnEnable()
    {
        input.SetPlayerMovement();
        input.MoveEvent += HandleMove;
        input.JumpEvent += HandleJump;
        input.JumpCancelledEvent += HandleCancelledJump;
    }
    void OnDisable()
    {
        input.MoveEvent -= HandleMove;
        input.JumpEvent -= HandleJump;
        input.JumpCancelledEvent -= HandleCancelledJump;
    }

    void Update()
    {
        Move();
    }

    private void HandleMove(Vector2 direction)
    {
        moveDirection = direction;
    }
    private void HandleJump()
    {
        isJumping = true;
        rb.velocity = new Vector2(rb.velocity.y, jumpPower);
    }

    private void HandleCancelledJump()
    {
        isJumping = false;
    }

    private void Move()
    {

        rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
    }

}
