using System.Collections;
using UnityEngine;

namespace PLAYER
{
    public class PlayerDash : MonoBehaviour
    {
        [Header("Dash Settings")]
        [SerializeField] private float dashingVelocity = 18f;
        [SerializeField] private float dashDuration = 0.15f;
        [SerializeField] private float postDashInputLock = 0.12f;
        [SerializeField] private int postDashIgnoreFrames = 2;

        private Rigidbody2D rb;

        public bool IsDashing { get; private set; }
        private bool canDash = true;
        private Vector2 dashDirection;
        private float postDashLockUntil;
        private int postDashIgnoreFramesCounter;
        public float PostDashLockUntil => postDashLockUntil;
        public int PostDashIgnoreFramesCounter => postDashIgnoreFramesCounter;
        private PlayerMovement movement;

        public void Initialize(Rigidbody2D rbRef, PlayerMovement movementRef)
        {
            rb = rbRef;
            movement = movementRef;
        }

        public void Tick(bool grounded)
        {
            if (grounded && !IsDashing)
                canDash = true;

            if (postDashIgnoreFramesCounter > 0)
                postDashIgnoreFramesCounter--;
        }

        public void FixedTick()
        {
            if (IsDashing)
            {
                canDash = false;
                rb.velocity = dashDirection * dashingVelocity;

                return;
            }
        }

        public void OnDash()
        {
            if (canDash)
            {
                dashDirection = movement.facingDirection;
                rb.velocity = Vector2.zero;
                IsDashing = true;
                canDash = false;

                postDashLockUntil = Time.time + postDashInputLock;
                postDashIgnoreFramesCounter = postDashIgnoreFrames;

                StartCoroutine(StopDashing());
            }
        }

        private IEnumerator StopDashing()
        {
            rb.gravityScale = 0;

            yield return new WaitForSeconds(dashDuration);

            rb.gravityScale = 1;
            IsDashing = false;

            postDashLockUntil = Time.time + postDashInputLock;
            postDashIgnoreFramesCounter = postDashIgnoreFrames;
        }
    }
}
