using System.Collections;
using UnityEngine;

namespace MOVEMENT
{
    public class PlayerController : MonoBehaviour
    {
        public Rigidbody2D rb;
        public float moveSpeed = 7f;
        public float jumpPower = 10f;
        [SerializeField] private InputReader input;
        [SerializeField] private LayerMask mask;

        private Vector2 moveDirection;
        private Vector2 playerSize;
        private Vector2 boxSize;


        [Header("Horizontal Movement")]
        private float acceleration = 120f;
        private float groundDecelerate = 60f;
        private float airDecelerate = 30f;
        private Vector2 facingDirection;

        [Header("Jump")]
        private float fallMultiplier = 7f;
        private float lowJumpMultiplier = 7f;
        private float jumpVelocityFallOff = 8f;
        private bool isGrounded;
        private int maxJumps = 1;
        private int jumpsRemaining;
        private bool isJumpHeld;
        private float groundedThreshold = 0.05f;
        [Header("Coyote Time and Jump Buffer")]
        private float coyoteTime = 0.2f;
        private float coyoteCounter;
        private float jumpBufferTime = 0.2f;
        private float jumpBufferCounter;

        [Header("Dashing")]
        private float dashingVelocity = 20f;
        private float dashingTime = 0.5f;
        private Vector2 dashingDir;
        private bool isDashing;
        private bool canDash = true;

        [Header("Interaction")]
        [SerializeField] private InteractionDetector interaction;


        void Awake()
        {
            playerSize = GetComponent<BoxCollider2D>().size;
            interaction = GetComponentInChildren<InteractionDetector>();
            boxSize = new Vector2(playerSize.x * 0.8f, groundedThreshold);
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
        }
        void FixedUpdate()
        {

            Move();

            if (isGrounded)
            {
                coyoteCounter = coyoteTime;
                canDash = true;
                jumpsRemaining = maxJumps;

            }
            else
                coyoteCounter -= Time.deltaTime;

            if (jumpBufferCounter > 0)
                jumpBufferCounter -= Time.deltaTime;

            if (coyoteCounter > 0f && jumpBufferCounter > 0f && isGrounded)
                Jump();

            // if (jumpBufferCounter > 0f && jumpsRemaining > 0)
            //     Jump();


            if (isDashing)
            {
                rb.velocity = facingDirection.normalized * dashingVelocity;
                return;
            }




            Vector2 boxMidPoint = (Vector2)transform.position + Vector2.down * (playerSize.y + boxSize.y) * 0.5f;
            isGrounded = (Physics2D.OverlapBox(boxMidPoint, boxSize, 0, mask) != null);
            Falling();



        }

        #region Handlers
        private void HandleMove(Vector2 direction)
        {
            moveDirection = direction;
            if (direction != Vector2.zero)
                facingDirection = direction;
        }
        private void HandleJumpPressed()
        {
            jumpBufferCounter = jumpBufferTime;
        }
        private void HandleJump()
        {
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

            TurnCheck();
            // rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
            if (moveDirection == Vector2.zero)
            {
                float deceleration = isGrounded ? groundDecelerate : airDecelerate;
                rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0, deceleration * Time.deltaTime), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, moveDirection.x * moveSpeed, acceleration * Time.deltaTime), rb.velocity.y);

            }
        }
        private void TurnCheck()
        {
            if (moveDirection == Vector2.zero)
                return;

            if (moveDirection.x < 0)
                GetComponent<SpriteRenderer>().flipX = true;
            else if (moveDirection.x > 0)
                GetComponent<SpriteRenderer>().flipX = false;
        }
        #endregion

        #region Jump
        private void Jump()
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            jumpBufferCounter = 0;
            isGrounded = false;
            jumpsRemaining--;

        }
        private void Falling()
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
                rb.velocity = Vector2.zero;
                isDashing = true;
                canDash = false;

                // forcefully set isGrounded here to false after we dash
                // idk the cause on why canDash is still true after dashing, but after i jump and dash then it consumes it
                isGrounded = false;

                StartCoroutine(StopDashing());
            }
        }
        private IEnumerator StopDashing()
        {
            rb.gravityScale = 0;
            input.SetUI();
            yield return new WaitForSeconds(0.15f);
            rb.gravityScale = 1;
            input.SetPlayerMovement();
            isDashing = false;
        }

        #endregion

        #region Interaction

        public void OnInteract()
        {
            Debug.Log("Interacti");
            interaction.interactableRange?.Interact();
        }
        #endregion



    }

}