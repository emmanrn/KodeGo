using UnityEngine;

public class EnemyFollower : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 0.5f;
    
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 10f;
    
    [Header("Visual Settings")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool rollLikeBall = true;
    [SerializeField] private float rotationSpeed = 360f; // Degrees per second at speed 1
    
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip chaseMusic;
    
    private bool isChasing = false;
    
    private Transform player;
    private Rigidbody2D rb;
    
    void Start()
    {
        // Find the player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        rb = GetComponent<Rigidbody2D>();
        
        // Get SpriteRenderer if not assigned
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        // Get AudioSource if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        // Configure AudioSource for music
        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        // Calculate distance to player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Only follow if within detection range and not too close
        if (distanceToPlayer < detectionRange && distanceToPlayer > stoppingDistance)
        {
            FollowPlayer();
        }
        else
        {
            // Stop moving
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }
        }
    }
    
    void FollowPlayer()
    {
        // Calculate direction to player
        Vector2 direction = (player.position - transform.position).normalized;
        
        // Move towards player
        if (rb != null)
        {
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            transform.position = Vector2.MoveTowards(
                transform.position, 
                player.position, 
                moveSpeed * Time.deltaTime
            );
        }
        
        // Roll the sprite like a ball
        if (rollLikeBall && spriteRenderer != null)
        {
            // Calculate rotation based on movement speed and direction
            float rotationAmount = rotationSpeed * moveSpeed * Time.deltaTime;
            
            // Rotate in the direction of movement
            // Positive X movement = clockwise, Negative X = counter-clockwise
            if (direction.x != 0)
            {
                transform.Rotate(0, 0, -direction.x * rotationAmount);
            }
        }
        else if (spriteRenderer != null)
        {
            // Optional: Flip sprite based on direction (if not rolling)
            if (direction.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (direction.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
    }
    
    // Visualize detection range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}