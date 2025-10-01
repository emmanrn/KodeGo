using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace MOVEMENT
{
    public class PlayerController : MonoBehaviour
    {
        public Rigidbody2D rb;
        public float moveSpeed = 7f;
        public float jumpPower = 10f;
        private bool isDying = false;
        [SerializeField] private InputReader input;
        [SerializeField] private LayerMask groundLayer;

        private Vector2 moveDirection;
        private Vector2 playerSize;
        private Vector2 boxSize;


        [Header("Horizontal Movement")]
        private float acceleration = 120f;
        private float groundDecelerate = 60f;
        private float airDecelerate = 30f;
        private Vector2 facingDirection;
        private bool isFacingRight = true;
        public bool isPressingDown { get { return moveDirection.y < -0.5f; } }

        [Header("Jump")]
        private float fallMultiplier = 7f;
        private float lowJumpMultiplier = 7f;
        private float jumpVelocityFallOff = 8f;
        private bool isJumpHeld;
        private float groundedThreshold = 0.05f;
        [Header("Coyote Time and Jump Buffer")]
        private float coyoteTime = 0.15f;
        private float coyoteCounter;
        private float jumpBufferTime = 0.2f;
        private float jumpBufferCounter;

        [Header("Dashing")]
        private float dashingVelocity = 18f;
        public bool isDashing { get; set; }
        private bool canDash = true;
        private Vector2 dashDirection;
        [SerializeField] private float postDashInputLock = 0.12f; // tweak
        [SerializeField] private int postDashIgnoreFrames = 2;    // robust across frames
        private float postDashLockUntil = 0f;
        private int postDashIgnoreFramesCounter = 0;

        [Header("Interaction")]
        private InteractionDetector interaction;

        [Header("Walls Movement and Checks")]
        [SerializeField] private Transform wallCheckPos;
        public Vector2 wallCheckSize = new Vector2(0.49f, 0.03f);
        public LayerMask wallLayer;
        private float wallSlideSpeed = 2f;
        private bool isWallSliding;
        private bool isWallJumping;
        private float wallJumpDir;
        private float wallJumpTime = 0.5f;
        private float wallJumpTimer;
        public Vector2 wallJumpPower = new Vector2(5f, 10f);

        [Header("Animations")]
        private Animator animator;

        [Header("Spawnpoint")]
        [SerializeField] private Transform startingPoint;
        [SerializeField] private CinemachineVirtualCamera roomCamera;

        [SerializeField] private CircleTransition circleTransition;



        void Awake()
        {
            playerSize = GetComponent<BoxCollider2D>().size;
            interaction = GetComponentInChildren<InteractionDetector>();
            boxSize = new Vector2(playerSize.x * 0.8f, groundedThreshold);
            animator = GetComponent<Animator>();
        }

        void OnEnable()
        {
            input.SetPlayerMovement();
            input.MoveEvent += HandleMove;
            input.JumpPressed += HandleJumpPressed;
            input.JumpEvent += HandleJump;
            input.JumpCancelledEvent += HandleCancelledJump;
            input.DashEvent += HandleDash;
            input.InteractEvent += OnInteract;
        }
        void OnDisable()
        {
            input.MoveEvent -= HandleMove;
            input.JumpEvent -= HandleJump;
            input.JumpCancelledEvent -= HandleCancelledJump;
            input.DashEvent -= HandleDash;
            input.InteractEvent -= OnInteract;
        }

        void Update()
        {
            if (postDashIgnoreFramesCounter > 0)
                postDashIgnoreFramesCounter--;
            if (isGrounded() && !isDashing)
            {
                coyoteCounter = coyoteTime;
                canDash = true;
                isWallJumping = false;
            }
            else
                coyoteCounter -= Time.deltaTime;

            if (jumpBufferCounter > 0)
                jumpBufferCounter -= Time.deltaTime;

            if (coyoteCounter > 0f && jumpBufferCounter > 0f && !isDashing)
                Jump();

            if (wallJumpTimer > 0f && jumpBufferCounter > 0f && !isDashing)
                Jump();

            WallSlide();
            WallJump();
            animator.SetFloat("yVelocity", rb.velocity.y);
            animator.SetFloat("magnitude", rb.velocity.magnitude);

        }
        void FixedUpdate()
        {
            if (isDashing)
            {
                canDash = false;
                rb.velocity = dashDirection * dashingVelocity;

                return;
            }

            Gravity();

            if (!isWallJumping)
            {
                Move();
                Flip();
            }

        }

        public bool isGrounded()
        {
            Vector2 boxMidPoint = (Vector2)transform.position + Vector2.down * (playerSize.y + boxSize.y) * 0.5f;
            return (Physics2D.OverlapBox(boxMidPoint, boxSize, 0, groundLayer) != null);

        }

        public bool isOnWall() => Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);


        #region Handlers
        private void HandleMove(Vector2 direction)
        {
            moveDirection = new Vector2(direction.x, direction.y);

            if (Mathf.Abs(moveDirection.x) > 0.01f)
                moveDirection.x = Mathf.Sign(moveDirection.x);
            if (direction != Vector2.zero)
                facingDirection = direction;


        }
        private void HandleJumpPressed()
        {
            if (isDashing || Time.time < postDashLockUntil || postDashIgnoreFramesCounter > 0) return;
            jumpBufferCounter = jumpBufferTime;
        }
        private void HandleJump()
        {
            if (isDashing || Time.time < postDashLockUntil || postDashIgnoreFramesCounter > 0) return;
            isJumpHeld = true;

        }

        private void HandleCancelledJump()
        {
            isJumpHeld = false;
            coyoteCounter = 0f;
        }
        private void HandleDash()
        {
            Dash();
        }
        #endregion

        #region Movement
        private void Move()
        {

            Flip();
            // rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
            float targetSpeed = moveDirection.x * moveSpeed;
            if (Mathf.Abs(moveDirection.x) < 0.01f)
            {
                float deceleration = isGrounded() ? groundDecelerate : airDecelerate;
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
        #endregion

        #region Jump
        private void Jump()
        {
            if (isDashing)
            {
                jumpBufferCounter = 0f; // flush any buffered jump that might have snuck through
                return;
            }
            Debug.Log("Jump called");
            // this is for the wall jump
            if (!isGrounded() && wallJumpTimer > 0)
            {
                isWallJumping = true;
                rb.velocity = new Vector2(wallJumpDir * wallJumpPower.x, wallJumpPower.y);
                wallJumpTimer = 0;
                animator.SetTrigger("Jump");

                if (transform.localScale.x != wallJumpDir)
                {
                    isFacingRight = !isFacingRight;
                    Vector3 ls = transform.localScale;
                    ls.x *= -1f;
                    transform.localScale = ls;
                }

                // wall jump would last for 0.5s
                // then we can jump again after 0.6s
                Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
                return;
            }

            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            jumpBufferCounter = 0f;
            animator.SetTrigger("Jump");

            // test to see if the double jump bug thing with coyote timer is fixed
            // jumpsRemaining--;


        }
        private void Gravity()
        {
            // if we are falling
            if (rb.velocity.y < 0)
                rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Vector2.up * Time.deltaTime;
            // if we are jumping up and we're not holding the jump button anymore just sshort jump
            else if (rb.velocity.y < jumpVelocityFallOff || rb.velocity.y > 0 && !isJumpHeld)
                rb.velocity += (lowJumpMultiplier - 1) * Physics2D.gravity.y * Vector2.up * Time.deltaTime;
            else
                rb.gravityScale = 1f;
        }

        #endregion

        #region Dash
        private void Dash()
        {
            if (canDash)
            {
                dashDirection = facingDirection.normalized;
                rb.velocity = Vector2.zero;
                isDashing = true;
                canDash = false;

                postDashLockUntil = Time.time + postDashInputLock;
                postDashIgnoreFramesCounter = postDashIgnoreFrames;

                StartCoroutine(StopDashing());
            }
        }
        private IEnumerator StopDashing()
        {
            rb.gravityScale = 0;
            yield return new WaitForSeconds(0.15f);
            rb.gravityScale = 1;
            isDashing = false;


            postDashLockUntil = Time.time + postDashInputLock;
            postDashIgnoreFramesCounter = postDashIgnoreFrames;


        }

        #endregion

        #region Interaction

        public void OnInteract()
        {
            interaction.interactableRange?.Interact();
        }
        #endregion

        #region Wall Jump
        private void WallSlide()
        {
            if (!isGrounded() && isOnWall() && moveDirection.x != 0)
            {
                isWallSliding = true;
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
            }
            else
                isWallSliding = false;
        }

        private void WallJump()
        {
            if (isWallSliding)
            {
                isWallJumping = false;
                wallJumpDir = -transform.localScale.x;
                wallJumpTimer = wallJumpTime;

                CancelInvoke(nameof(CancelWallJump));
            }
            else if (wallJumpTimer > 0f)
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }

        private void CancelWallJump()
        {
            isWallJumping = false;
        }

        #endregion


        #region Player Respawn
        public void Die()
        {
            if (isDying) return;
            StartCoroutine(Respawn());
        }
        public IEnumerator Respawn()
        {
            isDying = true;
            input.SetUI();
            // this will make the player stop and stay where they died so it can play a death animation
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            Color alpha = this.GetComponent<SpriteRenderer>().color;
            alpha.a = 1;

            this.GetComponent<SpriteRenderer>().color = alpha;

            yield return StartCoroutine(circleTransition.CloseCircle());
            transform.position = startingPoint.position;

            alpha.a = 1;
            this.GetComponent<SpriteRenderer>().color = alpha;

            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            input.SetPlayerMovement();

            CameraManager.SwitchCamera(roomCamera);

            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(circleTransition.OpenCircle());
            isDying = false;
        }
        #endregion 
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
        }



    }

}