using UnityEngine;

// Attach this script to each Room GameObject
public class RoomFlyingCars : MonoBehaviour
{
    [Header("Car Sprites")]
    [Tooltip("Drag your car sprites here - one will be randomly chosen each spawn")]
    public Sprite[] carSprites;
    
    [Header("Sound Effect")]
    [Tooltip("Sound effect to play when car appears (e.g., zoom/whoosh sound)")]
    public AudioClip carSpawnSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.5f;
    [Tooltip("AudioSource to use for playing sounds (for volume control)")]
    public AudioSource audioSource;
    
    [Header("Car Customization")]
    public Color carTint = Color.white;
    [Range(0.5f, 3f)]
    public float carScale = 1f;
    public float minSpeed = 25f;
    public float maxSpeed = 50f;
    
    [Header("Spawn Settings")]
    [Range(1f, 10f)]
    public float spawnInterval = 3f;
    [Tooltip("How many cars can exist at once")]
    public int maxCarsInRoom = 5;
    
    [Header("Room Size")]
    public float roomWidth = 25f;
    public float roomHeight = 15f;
    
    [Header("Height Range")]
    [Tooltip("Min height relative to room center")]
    public float minHeight = -5f;
    [Tooltip("Max height relative to room center")]
    public float maxHeight = 5f;
    
    [Header("Direction")]
    public bool spawnFromLeft = true;
    public bool spawnFromRight = true;
    
    [Header("Camera Settings")]
    public Camera mainCamera;
    [Tooltip("Extra margin to start spawning before camera fully enters room")]
    public float cameraActivationMargin = 5f;
    
    [Header("Debug")]
    public bool showDebugLogs = false;
    
    private float spawnTimer;
    private int activeCars = 0;
    private bool isRoomActive = false;
    
    void Start()
    {
        // Auto-find main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (mainCamera == null)
        {
            Debug.LogError("RoomFlyingCars: No camera found!");
            enabled = false;
            return;
        }
        
        if (carSprites == null || carSprites.Length == 0)
        {
            Debug.LogWarning("RoomFlyingCars: No car sprites assigned! Please assign at least one sprite in the Inspector.");
        }
        
        spawnTimer = spawnInterval; // Spawn first car immediately when room becomes active
    }
    
    void Update()
    {
        // Check if camera is in or near this room
        bool wasActive = isRoomActive;
        isRoomActive = IsCameraInRoom();
        
        if (isRoomActive && !wasActive && showDebugLogs)
        {
            Debug.Log($"Room '{gameObject.name}' activated - cars will spawn");
        }
        else if (!isRoomActive && wasActive && showDebugLogs)
        {
            Debug.Log($"Room '{gameObject.name}' deactivated - cars will stop spawning");
        }
        
        // Only spawn cars if room is active
        if (!isRoomActive) return;
        
        spawnTimer += Time.deltaTime;
        
        if (spawnTimer >= spawnInterval && activeCars < maxCarsInRoom)
        {
            SpawnCar();
            spawnTimer = 0f;
        }
    }
    
    bool IsCameraInRoom()
    {
        if (mainCamera == null) return false;
        
        // Get camera bounds
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        
        Vector3 camPos = mainCamera.transform.position;
        Vector3 roomPos = transform.position;
        
        // Check if camera overlaps with room (with margin)
        float roomLeft = roomPos.x - (roomWidth / 2f) - cameraActivationMargin;
        float roomRight = roomPos.x + (roomWidth / 2f) + cameraActivationMargin;
        float roomBottom = roomPos.y - (roomHeight / 2f) - cameraActivationMargin;
        float roomTop = roomPos.y + (roomHeight / 2f) + cameraActivationMargin;
        
        float camLeft = camPos.x - (cameraWidth / 2f);
        float camRight = camPos.x + (cameraWidth / 2f);
        float camBottom = camPos.y - (cameraHeight / 2f);
        float camTop = camPos.y + (cameraHeight / 2f);
        
        // Check for overlap
        bool overlapsX = camRight > roomLeft && camLeft < roomRight;
        bool overlapsY = camTop > roomBottom && camBottom < roomTop;
        
        return overlapsX && overlapsY;
    }
    
    void SpawnCar()
    {
        if (carSprites == null || carSprites.Length == 0)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning("Cannot spawn car - no sprites assigned!");
            }
            return;
        }
        
        // Randomly choose a car sprite
        Sprite selectedSprite = carSprites[Random.Range(0, carSprites.Length)];
        
        if (selectedSprite == null)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning("Selected car sprite is null - skipping spawn");
            }
            return;
        }
        
        // Determine spawn direction
        bool fromLeft;
        if (spawnFromLeft && spawnFromRight)
        {
            fromLeft = Random.value > 0.5f;
        }
        else if (spawnFromLeft)
        {
            fromLeft = true;
        }
        else if (spawnFromRight)
        {
            fromLeft = false;
        }
        else
        {
            // Neither direction enabled
            return;
        }
        
        // Calculate spawn position relative to room
        Vector3 roomPos = transform.position;
        float spawnDistance = 3f; // Distance outside room edge
        
        float xPos = fromLeft ? 
            roomPos.x - (roomWidth / 2f) - spawnDistance :
            roomPos.x + (roomWidth / 2f) + spawnDistance;
        
        float yPos = roomPos.y + Random.Range(minHeight, maxHeight);
        
        Vector3 spawnPos = new Vector3(xPos, yPos, roomPos.z);
        
        // Create car GameObject
        GameObject carObj = new GameObject("FlyingCar");
        carObj.transform.position = spawnPos;
        
        // Add sprite renderer
        SpriteRenderer sr = carObj.AddComponent<SpriteRenderer>();
        sr.sprite = selectedSprite;
        sr.color = carTint;
        sr.sortingOrder = 50; // In front of background, behind player
        
        // Set scale
        carObj.transform.localScale = Vector3.one * carScale;
        
        // Flip if moving left
        if (!fromLeft)
        {
            carObj.transform.localScale = new Vector3(-carScale, carScale, carScale);
        }
        
        // Add movement script
        FlyingCar carScript = carObj.AddComponent<FlyingCar>();
        float speed = Random.Range(minSpeed, maxSpeed);
        carScript.Initialize(speed, fromLeft, mainCamera, this, carSpawnSound, soundVolume, audioSource);
        
        activeCars++;
        
        if (showDebugLogs)
        {
            Debug.Log($"Car spawned at {spawnPos}, moving {(fromLeft ? "right" : "left")} at speed {speed}. Active cars: {activeCars}");
        }
    }
    
    public void OnCarDestroyed()
    {
        activeCars--;
        if (activeCars < 0) activeCars = 0;
    }
    
    // Visualize the room bounds and spawn areas in the Scene view
    void OnDrawGizmosSelected()
    {
        // Draw room bounds
        Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.3f);
        Gizmos.DrawWireCube(transform.position, new Vector3(roomWidth, roomHeight, 1f));
        
        // Draw height range
        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        Vector3 minHeightPos = transform.position + new Vector3(0f, minHeight, 0f);
        Vector3 maxHeightPos = transform.position + new Vector3(0f, maxHeight, 0f);
        Gizmos.DrawLine(minHeightPos - Vector3.right * (roomWidth / 2f), minHeightPos + Vector3.right * (roomWidth / 2f));
        Gizmos.DrawLine(maxHeightPos - Vector3.right * (roomWidth / 2f), maxHeightPos + Vector3.right * (roomWidth / 2f));
        
        // Draw spawn positions
        float spawnDist = 3f;
        if (spawnFromLeft)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.7f);
            Vector3 leftSpawn = transform.position + new Vector3(-roomWidth / 2f - spawnDist, 0f, 0f);
            Gizmos.DrawWireSphere(leftSpawn, 1f);
            Gizmos.DrawLine(leftSpawn, leftSpawn + Vector3.right * 2f);
        }
        
        if (spawnFromRight)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.7f);
            Vector3 rightSpawn = transform.position + new Vector3(roomWidth / 2f + spawnDist, 0f, 0f);
            Gizmos.DrawWireSphere(rightSpawn, 1f);
            Gizmos.DrawLine(rightSpawn, rightSpawn + Vector3.left * 2f);
        }
        
        // Draw camera activation bounds
        if (mainCamera != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f);
            float activationWidth = roomWidth + (cameraActivationMargin * 2f);
            float activationHeight = roomHeight + (cameraActivationMargin * 2f);
            Gizmos.DrawWireCube(transform.position, new Vector3(activationWidth, activationHeight, 1f));
        }
    }
}

public class FlyingCar : MonoBehaviour
{
    private float speed;
    private bool movingRight;
    private Camera mainCam;
    private RoomFlyingCars roomScript;
    private AudioClip spawnSound;
    private float soundVolume;
    private AudioSource audioSource;
    private bool hasPlayedSound = false;
    
    public void Initialize(float carSpeed, bool fromLeft, Camera cam, RoomFlyingCars room, AudioClip sound, float volume, AudioSource source)
    {
        speed = carSpeed;
        movingRight = fromLeft;
        mainCam = cam;
        roomScript = room;
        spawnSound = sound;
        soundVolume = volume;
        audioSource = source;
    }
    
    void Update()
    {
        if (mainCam == null)
        {
            Destroy(gameObject);
            return;
        }
        
        // Check if car is on screen and play sound once
        if (!hasPlayedSound && IsOnScreen())
        {
            PlaySpawnSound();
            hasPlayedSound = true;
        }
        
        // Move the car
        float direction = movingRight ? 1f : -1f;
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime, Space.World);
        
        // Check if car is off screen and destroy it
        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);
        
        // Add extra margin for destruction
        if (viewportPos.x < -0.2f || viewportPos.x > 1.2f)
        {
            if (roomScript != null)
            {
                roomScript.OnCarDestroyed();
            }
            Destroy(gameObject);
        }
    }
    
    bool IsOnScreen()
    {
        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);
        return viewportPos.x >= 0f && viewportPos.x <= 1f && 
               viewportPos.y >= 0f && viewportPos.y <= 1f;
    }
    
    void PlaySpawnSound()
    {
        if (spawnSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(spawnSound, soundVolume);
        }
    }
}