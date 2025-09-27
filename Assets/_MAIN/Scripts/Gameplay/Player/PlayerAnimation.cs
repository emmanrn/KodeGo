using UnityEngine;

namespace PLAYER
{
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator animator;
        private Rigidbody2D rb;

        public void Initialize(Rigidbody2D rbRef)
        {
            rb = rbRef;
            animator = GetComponent<Animator>();
        }

        public void Tick()
        {
            if (!animator) return;
            animator.SetFloat("yVelocity", rb.velocity.y);
            animator.SetFloat("magnitude", rb.velocity.magnitude);
        }

        public void Jump() => animator.SetTrigger("Jump");
    }
}
