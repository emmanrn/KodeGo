using UnityEngine;

namespace PLAYER
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundedThreshold = 0.05f;
        private Vector2 boxSize;
        private Vector2 playerSize;


        private void Awake()
        {
            playerSize = GetComponent<BoxCollider2D>().size;
            boxSize = new Vector2(playerSize.x * 0.9f, groundedThreshold);
        }

        public bool isGrounded()
        {
            Vector2 boxMidPoint = (Vector2)transform.position + Vector2.down * (playerSize.y + boxSize.y) * 0.5f;
            return (Physics2D.OverlapBox(boxMidPoint, boxSize, 0, groundLayer) != null);

        }

    }
}

