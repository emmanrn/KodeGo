using System;
using UnityEngine;

namespace PLAYER
{
    public class PlayerJump : MonoBehaviour
    {
        [Header("Jump Settings")]
        [SerializeField] private float jumpPower = 10f;
        [SerializeField] private float fallMultiplier = 7f;
        [SerializeField] private float lowJumpMultiplier = 7f;
        [SerializeField] private float jumpVelocityFallOff = 8f;

        [Header("Coyote & Buffer")]
        [SerializeField] private float coyoteTime = 0.15f;
        [SerializeField] private float jumpBufferTime = 0.2f;

        private Rigidbody2D rb;
        private PlayerWall wallJump;

        private float coyoteCounter;
        private float jumpBufferCounter;
        private bool isJumpHeld;
        private bool jumpQueued;
        public bool IsJumpBuffered => jumpBufferCounter > 0f;
        private Action jump;

        public void Initialize(Rigidbody2D rbRef, PlayerWall wallJumpRef, System.Action jump)
        {
            rb = rbRef;
            wallJump = wallJumpRef;
            this.jump = jump;
        }

        public void Tick(bool grounded)
        {
            if (grounded)
                coyoteCounter = coyoteTime;
            else
            {
                coyoteCounter -= Time.deltaTime;
            }

            if (jumpBufferCounter > 0)
                jumpBufferCounter -= Time.deltaTime;

            if (coyoteCounter > 0f && jumpBufferCounter > 0f)
            {
                jumpQueued = true;
                jumpBufferCounter = 0f; // consume buffer
                jump();
            }

        }

        public void FixedTick()
        {
            ApplyGravity();

            if (jumpQueued)
            {
                Jump();
                jumpQueued = false;
            }
        }

        public void OnJumpPressed() => jumpBufferCounter = jumpBufferTime;
        public void OnJumpHeld() => isJumpHeld = true;
        public void OnJumpReleased()
        {
            isJumpHeld = false;
            coyoteCounter = 0f;
        }

        public void Jump()
        {
            rb.gravityScale = 1f;
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            jumpBufferCounter = 0f;
        }

        public void ClearJumpBuffer()
        {
            jumpBufferCounter = 0f;
            isJumpHeld = false;
        }

        private void ApplyGravity()
        {
            if (rb.velocity.y < 0)
                rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Vector2.up * Time.deltaTime;
            // if we are jumping up and we're not holding the jump button anymore just sshort jump
            else if (rb.velocity.y < jumpVelocityFallOff || rb.velocity.y > 0 && !isJumpHeld)
                rb.velocity += (lowJumpMultiplier - 1) * Physics2D.gravity.y * Vector2.up * Time.deltaTime;
            else
                rb.gravityScale = 1f;
        }
    }
}
