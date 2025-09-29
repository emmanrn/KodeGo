using UnityEngine;

namespace PLAYER
{
    public class PlayerWall : MonoBehaviour
    {
        [Header("Wall Settings")]
        [SerializeField] private Transform wallCheckPos;
        [SerializeField] private Vector2 wallCheckSize = new(0.49f, 0.03f);
        [SerializeField] private LayerMask wallLayer;
        [SerializeField] private Vector2 wallJumpPower = new(5f, 10f);
        [SerializeField] private float wallSlideSpeed = 2f;
        [SerializeField] private float wallJumpTime = 0.5f;

        private Rigidbody2D rb;

        private bool isWallSliding;
        public bool IsWallJumping { get; private set; }
        public bool CanWallJump { get; private set; }
        private float wallJumpDir;
        private float wallJumpTimer;
        public float WallJumpTimer => wallJumpTimer;

        public void Initialize(Rigidbody2D rbRef)
        {
            rb = rbRef;
        }

        public void Tick(Vector2 moveDir, bool grounded)
        {
            WallSlide(moveDir, grounded);
            WallJump();
        }

        private void WallSlide(Vector2 moveDir, bool grounded)
        {
            if (!grounded && IsOnWall() && moveDir.x != 0)
            {
                isWallSliding = true;
                CanWallJump = true;
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
            }
            else
            {
                isWallSliding = false;
                CanWallJump = false;

            }
        }

        public void WallJump()
        {
            if (isWallSliding)
            {
                IsWallJumping = false;
                wallJumpDir = -transform.localScale.x;
                wallJumpTimer = wallJumpTime;

                CancelInvoke(nameof(CancelWallJump));
            }
            else if (wallJumpTimer > 0f)
                wallJumpTimer -= Time.deltaTime;
        }
        public void TriggerWallJump()
        {
            if (wallJumpTimer > 0)
            {
                IsWallJumping = true;
                // Determine the jump direction from the wall
                rb.velocity = new Vector2(wallJumpDir * wallJumpPower.x, wallJumpPower.y);
                wallJumpTimer = 0;

                // Optionally flip the character's facing if needed
                if (transform.localScale.x != wallJumpDir)
                {
                    Vector3 ls = transform.localScale;
                    ls.x *= -1f;
                    transform.localScale = ls;
                }

                // Prevent immediate re-triggering of a wall jump
                Invoke(nameof(CancelWallJump), 0.6f);
            }
        }

        private void CancelWallJump() => IsWallJumping = false;

        private bool IsOnWall() =>
            Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
        }
    }
}
