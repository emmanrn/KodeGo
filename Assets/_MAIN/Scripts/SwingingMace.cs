using UnityEngine;

public class SwingingMace : MonoBehaviour
{
    [Header("Swing Settings")]
    [SerializeField] private float swingAngle = 45f; // Maximum swing angle in degrees
    [SerializeField] private float swingSpeed = 2f; // Speed of the swing
    
    [Header("Chain Settings")]
    [SerializeField] private int chainLinks = 8; // Number of chain links
    [SerializeField] private float chainLinkSize = 0.5f; // Size of each chain link
    [SerializeField] private Sprite chainLinkSprite; // Sprite for chain links
    
    [Header("Mace Settings")]
    [SerializeField] private Sprite maceSprite; // Sprite for the mace head
    [SerializeField] private float maceSize = 1f; // Size of the mace head
    
    private float currentAngle = 0f;
    private float direction = 1f; // 1 for right, -1 for left
    private GameObject[] chainLinkObjects;
    private GameObject maceObject;
    
    void Start()
    {
        CreateChainAndMace();
    }
    
    void CreateChainAndMace()
    {
        // Create chain links
        chainLinkObjects = new GameObject[chainLinks];
        
        for (int i = 0; i < chainLinks; i++)
        {
            GameObject link = new GameObject($"ChainLink_{i}");
            link.transform.parent = transform;
            link.transform.localPosition = new Vector3(0, -(i + 1) * chainLinkSize, 0);
            
            SpriteRenderer sr = link.AddComponent<SpriteRenderer>();
            sr.sprite = chainLinkSprite;
            sr.sortingOrder = 1;
            
            // Scale the chain link
            link.transform.localScale = new Vector3(chainLinkSize, chainLinkSize, 1);
            
            chainLinkObjects[i] = link;
        }
        
        // Create mace at the end of the chain
        maceObject = new GameObject("Mace");
        maceObject.transform.parent = transform;
        maceObject.transform.localPosition = new Vector3(0, -(chainLinks + 1) * chainLinkSize, 0);
        
        SpriteRenderer maceSr = maceObject.AddComponent<SpriteRenderer>();
        maceSr.sprite = maceSprite;
        maceSr.sortingOrder = 2;
        
        // Scale the mace
        maceObject.transform.localScale = new Vector3(maceSize, maceSize, 1);
        
        // Add collider and Spikes script to mace
        CircleCollider2D maceCollider = maceObject.AddComponent<CircleCollider2D>();
        maceCollider.isTrigger = true;
        maceCollider.radius = 3f; // Smaller hitbox (adjust as needed)
        maceObject.AddComponent<Spikes>();
    }
    
    void Update()
    {
        // Calculate the swing
        currentAngle += swingSpeed * direction * Time.deltaTime * 100f;
        
        // Reverse direction when reaching the swing limits
        if (currentAngle >= swingAngle)
        {
            currentAngle = swingAngle;
            direction = -1f;
        }
        else if (currentAngle <= -swingAngle)
        {
            currentAngle = -swingAngle;
            direction = 1f;
        }
        
        // Apply rotation (rotates around Z axis for 2D)
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
}

/* 
SETUP INSTRUCTIONS:
1. Create a new GameObject in your scene (this will be the wall attachment point)
2. Add this script to that GameObject
3. In the Inspector, assign:
   - Chain Link Sprite (a small circular or rectangular sprite)
   - Mace Sprite (your mace head sprite)
4. Adjust the settings:
   - Chain Links: How many links in the chain
   - Chain Link Size: Size of each link
   - Mace Size: Size of the mace head
   - Swing Angle & Speed: How it swings
5. Hit play!

NOTE: If you don't have sprites ready, you can create simple ones:
- Chain Link: Small white circle or square
- Mace: Circle with spikes or any mace design
The script will automatically create and position everything for you!
*/