using UnityEngine;

namespace PLAYER
{
    public class PlayerState : MonoBehaviour
    {
        // Movement states
        public bool IsGrounded { get; set; }
        public bool IsDashing { get; set; }
        public bool IsWallSliding { get; set; }
        public bool IsWallJumping { get; set; }

        // Movement values
        public float YVelocity { get; set; }
        public float Speed { get; set; }

        // Facing direction
        public bool IsFacingRight { get; set; } = true;
    }
}
