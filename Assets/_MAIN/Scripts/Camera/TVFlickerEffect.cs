using UnityEngine;
using Cinemachine;

// Attach this script to each Room GameObject (alongside RoomSnowEffect)
public class TVFlickerEffect : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private AnimationClip flickerClip;
    [SerializeField] private bool flickerOnRoomEnter = true;
    
    [Header("Sprite Setup (Optional)")]
    [SerializeField] private SpriteRenderer flickerSprite;
    [SerializeField] private int sortingOrder = 1000;
    
    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera roomVirtualCamera;
    
    private bool hasFlickered = false;
    private CinemachineBrain cinemachineBrain;
    private Animation anim;
    
    private void Awake()
    {
        // Auto-find main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        // Find the Cinemachine brain
        if (mainCamera != null)
        {
            cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
        }
        
        // Try to find virtual camera in this room if not assigned
        if (roomVirtualCamera == null)
        {
            roomVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        }
        
        // Create sprite if not assigned
        if (flickerSprite == null)
        {
            CreateFlickerSprite();
        }
        
        // Add Animation component to the sprite
        if (flickerSprite != null)
        {
            anim = flickerSprite.gameObject.AddComponent<Animation>();
            anim.playAutomatically = false;
            
            // Add the clip if provided
            if (flickerClip != null)
            {
                // Make sure the clip is set to Legacy
                flickerClip.legacy = true;
                anim.AddClip(flickerClip, flickerClip.name);
                anim.clip = flickerClip;
                
                Debug.Log($"Added animation clip '{flickerClip.name}' to FlickerSprite. Clip length: {flickerClip.length}s");
            }
        }
        
        // Start with sprite hidden
        if (flickerSprite != null)
        {
            Color c = flickerSprite.color;
            c.a = 0;
            flickerSprite.color = c;
        }
    }
    
    private void Update()
    {
        // Check if this room's camera is active
        if (flickerOnRoomEnter && cinemachineBrain != null && roomVirtualCamera != null)
        {
            // Check if this room's virtual camera is the active one
            ICinemachineCamera activeCam = cinemachineBrain.ActiveVirtualCamera;
            
            if (activeCam != null && activeCam == roomVirtualCamera as ICinemachineCamera)
            {
                // Trigger flicker only once when entering the room
                if (!hasFlickered)
                {
                    Debug.Log($"Triggering flicker for room: {gameObject.name}");
                    TriggerFlicker();
                    hasFlickered = true;
                }
            }
            else
            {
                // Reset when leaving the room so it flickers again on re-entry
                hasFlickered = false;
            }
        }
    }
    
    private void LateUpdate()
    {
        // Keep the sprite covering the screen
        if (flickerSprite != null && mainCamera != null)
        {
            // Position at camera
            Vector3 pos = mainCamera.transform.position;
            pos.z = mainCamera.transform.position.z + mainCamera.nearClipPlane + 0.1f;
            flickerSprite.transform.position = pos;
            
            // Scale to cover screen
            float height = mainCamera.orthographicSize * 2f;
            float width = height * mainCamera.aspect;
            flickerSprite.transform.localScale = new Vector3(width, height, 1);
        }
    }
    
    private void CreateFlickerSprite()
    {
        // Create sprite object
        GameObject spriteObj = new GameObject("FlickerSprite_" + gameObject.name);
        flickerSprite = spriteObj.AddComponent<SpriteRenderer>();
        
        // Create a simple white square sprite
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        flickerSprite.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        
        // Set sorting
        flickerSprite.sortingOrder = sortingOrder;
        
        // Start transparent
        Color c = flickerSprite.color;
        c.a = 0;
        flickerSprite.color = c;
    }
    
    // Call this method to trigger the flicker animation
    public void TriggerFlicker()
    {
        if (anim == null)
        {
            Debug.LogError($"Animation component is null on {gameObject.name}");
            return;
        }
        
        if (flickerClip == null)
        {
            Debug.LogError($"Flicker clip is not assigned on {gameObject.name}");
            return;
        }
        
        Debug.Log($"Playing flicker animation: {flickerClip.name}");
        anim.Stop(); // Stop any currently playing animation
        anim.Play(flickerClip.name);
    }
    
    // Reset the flicker state (useful if you want to manually trigger it again)
    public void ResetFlicker()
    {
        hasFlickered = false;
    }
}