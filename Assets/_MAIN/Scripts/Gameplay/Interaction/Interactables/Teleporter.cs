using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Teleporter Settings")]
    [Tooltip("The destination teleporter to transport to")]
    public Transform destination;
    
    [Tooltip("Tag of the object that can use the teleporter (e.g., 'Player')")]
    public string teleportTag = "Player";
    
    [Tooltip("Cooldown time in seconds to prevent rapid re-teleporting")]
    public float cooldownTime = 0.5f;
    
    [Tooltip("Optional visual effect to spawn at teleport")]
    public GameObject teleportEffect;
    
    [Tooltip("Key to press to activate teleport")]
    public KeyCode activationKey = KeyCode.E;
    
    [Header("UI Indicator")]
    [Tooltip("UI Text or TextMeshPro component to show prompt")]
    public GameObject promptUI;
    
    [Tooltip("Message to display when player can teleport")]
    public string promptMessage = "Press E to Teleport";
    
    private float lastTeleportTime = -999f;
    private bool playerInRange = false;
    private Collider2D playerCollider;
    
    private void Start()
    {
        // Hide the prompt at start
        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(teleportTag))
        {
            playerInRange = true;
            playerCollider = other;
            
            // Show the prompt
            if (promptUI != null)
            {
                promptUI.SetActive(true);
            }
            
            Debug.Log("Press E to teleport");
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(teleportTag))
        {
            playerInRange = false;
            playerCollider = null;
            
            // Hide the prompt
            if (promptUI != null)
            {
                promptUI.SetActive(false);
            }
        }
    }
    
    private void Update()
    {
        // Check if player is in range and presses the activation key
        if (playerInRange && Input.GetKeyDown(activationKey) && Time.time - lastTeleportTime > cooldownTime)
        {
            if (destination != null && playerCollider != null)
            {
                // Spawn effect at current position
                if (teleportEffect != null)
                {
                    Instantiate(teleportEffect, playerCollider.transform.position, Quaternion.identity);
                }
                
                // Teleport the object
                playerCollider.transform.position = destination.position;
                
                // Spawn effect at destination
                if (teleportEffect != null)
                {
                    Instantiate(teleportEffect, destination.position, Quaternion.identity);
                }
                
                // Update cooldown on both teleporters if destination has a Teleporter component
                lastTeleportTime = Time.time;
                Teleporter destinationTeleporter = destination.GetComponent<Teleporter>();
                if (destinationTeleporter != null)
                {
                    destinationTeleporter.lastTeleportTime = Time.time;
                }
                
                Debug.Log($"Teleported {playerCollider.name} to {destination.name}");
            }
            else if (destination == null)
            {
                Debug.LogWarning("Destination not set on teleporter!");
            }
        }
    }
    
    // Visual helper in the editor
    private void OnDrawGizmos()
    {
        if (destination != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destination.position);
            Gizmos.DrawWireSphere(transform.position, 0.3f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(destination.position, 0.3f);
        }
    }
}