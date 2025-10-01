using UnityEngine;

namespace PLAYER
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private float acceleration = 120f;
        [SerializeField] private float groundDecelerate = 60f;
        [SerializeField] private float airDecelerate = 30f;

        private Rigidbody2D rb;
        private Vector2 moveDirection;
        public bool isFacingRight = true;
        public Vector2 facingDirection { get; set; }

        public Vector2 MoveDirection => moveDirection;
        public bool isPressingDown { get { return moveDirection.y < -0.5f; } }

        public void Initialize(Rigidbody2D rbRef) => rb = rbRef;

        public void SetMoveDirection(Vector2 dir)
        {
            moveDirection = new Vector2(dir.x, dir.y);

            if (Mathf.Abs(moveDirection.x) > 0.01f)
                moveDirection.x = Mathf.Sign(moveDirection.x);
            if (dir != Vector2.zero)
                facingDirection = dir;
        }

        public void FixedTick(bool grounded)
        {
            Move(grounded);
            Flip();
        }

        private void Move(bool grounded)
        {

            Flip();
            // rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
            float targetSpeed = moveDirection.x * moveSpeed;
            if (Mathf.Abs(moveDirection.x) < 0.01f)
            {
                float deceleration = grounded ? groundDecelerate : airDecelerate;
                rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0, deceleration * Time.deltaTime), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, targetSpeed, acceleration * Time.deltaTime), rb.velocity.y);

            }
        }
        private void Flip()
        {
            if (moveDirection == Vector2.zero)
                return;

            // this is whats making our player turn in the direction they're going 
            // by checking the moveDirection.x (horizontal movement) if it's less than then we're going left and greater then if we're going right
            // then just basically flip our chaaracter transform scale depending on direction
            if (isFacingRight && moveDirection.x < 0 || !isFacingRight && moveDirection.x > 0)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }
        }
    }
}