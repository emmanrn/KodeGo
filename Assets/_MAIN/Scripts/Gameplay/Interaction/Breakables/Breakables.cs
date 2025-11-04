
using PLAYER;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Breakables : MonoBehaviour
{
    [SerializeField] private Tilemap backgroundTilemap; // Assign your tilemap in inspector
    [SerializeField] private GameObject breakPiecePrefab; // Optional: custom piece sprite
    [SerializeField] private int numberOfPieces = 4; // How many pieces to spawn
    [SerializeField] private float pieceForce = 5f; // How hard pieces fly out
    [SerializeField] private float pieceGravity = 2f; // Gravity scale for pieces
    [SerializeField] private AudioClip breakSound; // Assign your break sound effect

    [Header("Break Conditions")]
    [SerializeField] private bool breakOnDash = true; // Break when player dashes into it
    [SerializeField] private bool breakOnJumpedOn = false; // Break when player jumps on top

    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayerController playerController = other.collider.GetComponent<PlayerController>();
        if (playerController != null)
        {
            bool shouldBreak = false;

            // Check if dashing and dash breaking is enabled
            if (breakOnDash && playerController.isDashing)
            {
                shouldBreak = true;
            }

            // Check if jumped on top and jump breaking is enabled
            if (breakOnJumpedOn && !playerController.isDashing)
            {
                // Check the contact point to see if player hit from above
                foreach (ContactPoint2D contact in other.contacts)
                {
                    // If the contact point is on the top side of the block (contact is below the player)
                    if (contact.point.y > transform.position.y)
                    {
                        shouldBreak = true;
                        break;
                    }
                }
            }

            if (shouldBreak)
            {
                // Play break sound
                if (breakSound != null)
                {
                    AudioSource.PlayClipAtPoint(breakSound, transform.position);
                }

                // Play break animation
                SpawnBreakPieces();

                // Remove the tile at this block's position AFTER spawning pieces
                if (backgroundTilemap != null)
                {
                    Vector3Int tilePosition = backgroundTilemap.WorldToCell(transform.position);
                    backgroundTilemap.SetTile(tilePosition, null);
                }

                this.gameObject.SetActive(false);
            }
        }
    }

    private void SpawnBreakPieces()
    {
        if (backgroundTilemap == null) return;

        // Get the tile position and sprite
        Vector3Int tilePosition = backgroundTilemap.WorldToCell(transform.position);
        Sprite tileSprite = backgroundTilemap.GetSprite(tilePosition);
        TilemapRenderer tilemapRenderer = backgroundTilemap.GetComponent<TilemapRenderer>();

        if (tileSprite == null) return;

        for (int i = 0; i < numberOfPieces; i++)
        {
            GameObject piece;

            if (breakPiecePrefab != null)
            {
                // Use custom prefab if assigned
                piece = Instantiate(breakPiecePrefab, transform.position, Quaternion.identity);
            }
            else
            {
                // Create simple square pieces
                piece = new GameObject("BreakPiece");
                piece.transform.position = transform.position;

                SpriteRenderer sr = piece.AddComponent<SpriteRenderer>();

                // Use the tilemap sprite
                sr.sprite = tileSprite;

                // Match tilemap sorting layer and order
                if (tilemapRenderer != null)
                {
                    sr.sortingLayerName = tilemapRenderer.sortingLayerName;
                    sr.sortingOrder = tilemapRenderer.sortingOrder + 1; // Slightly above so pieces show on top
                }

                // Make it smaller to look like a piece
                piece.transform.localScale = Vector3.one * 0.3f;
            }

            // Add physics
            Rigidbody2D rb = piece.AddComponent<Rigidbody2D>();
            rb.gravityScale = pieceGravity;

            // Launch in random directions (upward and outward like Mario)
            float angle = Random.Range(30f, 150f); // Upward angles
            float randomForce = Random.Range(pieceForce * 0.8f, pieceForce * 1.2f);
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            rb.velocity = direction * randomForce;

            // Add random spin
            rb.angularVelocity = Random.Range(-360f, 360f);

            // Destroy piece after 2 seconds
            Destroy(piece, 2f);
        }
    }
}