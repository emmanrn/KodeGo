using UnityEngine;

// Attach this script to each Room GameObject for water drop effects
public class RoomWaterDropEffect : MonoBehaviour
{
    [Header("Water Drop Customization")]
    [Range(-1f, 1f)]
    public float horizontalDrift = 0.1f; // Slight drift as they fall
    [Range(0.5f, 3f)]
    public float fallSpeed = 1.2f; // Slower than snow
    public Color waterColor = new Color(1f, 1f, 1f, 0.6f); // Pure white/clear water
    
    [Header("Room Size")]
    public float roomWidth = 25f;
    public float roomHeight = 15f;
    
    [Header("Particle Settings")]
    public float emissionRate = 5f; // More visible for testing
    public int maxParticles = 200;
    [Range(0.02f, 0.15f)]
    public float minParticleSize = 0.05f;
    [Range(0.15f, 0.4f)]
    public float maxParticleSize = 0.15f;
    
    [Header("Drop Behavior")]
    [Tooltip("Random variation in fall speed (0-1)")]
    [Range(0f, 0.5f)]
    public float speedVariation = 0.3f;
    [Tooltip("How stretched the drops are (1 = circle, >1 = vertical stretch)")]
    [Range(1f, 3f)]
    public float dropStretch = 1.8f;
    
    [Header("Camera Bounds")]
    [Tooltip("Particles outside camera view will be destroyed")]
    public Camera mainCamera;
    [Tooltip("Extra margin beyond camera bounds (in world units)")]
    public float cameraBoundsMargin = 2f;
    
    private ParticleSystem waterParticles;
    private ParticleSystem.Particle[] particles;
    
    void Start()
    {
        // Auto-find main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (mainCamera == null)
        {
            Debug.LogWarning("No camera found! Particles won't be culled.");
        }
        
        CreateWaterParticles();
        SpawnInitialDrops();
    }
    
    void CreateWaterParticles()
    {
        // Create particle system as child of room
        GameObject psObj = new GameObject("WaterDroplets");
        psObj.transform.SetParent(transform);
        
        // Position at the top of the room (ceiling)
        psObj.transform.localPosition = new Vector3(0f, roomHeight / 2f + 0.5f, 0f);
        
        waterParticles = psObj.AddComponent<ParticleSystem>();
        
        // Initialize particles array
        particles = new ParticleSystem.Particle[maxParticles];
        
        // Configure main module
        var main = waterParticles.main;
        main.startLifetime = 15f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.8f, 1.5f);
        main.startSpeedMultiplier = fallSpeed;
        main.startSize = new ParticleSystem.MinMaxCurve(minParticleSize, maxParticleSize);
        main.startSize3D = true;
        main.startSizeX = new ParticleSystem.MinMaxCurve(minParticleSize, maxParticleSize);
        main.startSizeY = new ParticleSystem.MinMaxCurve(minParticleSize * dropStretch, maxParticleSize * dropStretch);
        main.startSizeZ = new ParticleSystem.MinMaxCurve(minParticleSize, maxParticleSize);
        main.startColor = waterColor;
        main.gravityModifier = 0.8f; // More gravity than snow
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = maxParticles;
        main.startRotation = 0f; // Drops don't rotate
        main.loop = true;
        main.prewarm = false;
        
        // Configure emission - sparse and irregular
        var emission = waterParticles.emission;
        emission.rateOverTime = emissionRate;
        
        // Add bursts for occasional drip clusters
        emission.burstCount = 0; // No bursts - just steady rare drops
        
        // Configure shape - Rectangle spanning room width
        var shape = waterParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Rectangle;
        shape.scale = new Vector3(roomWidth * 0.8f, 1f, 1f); // Slightly narrower
        shape.rotation = new Vector3(90f, 0f, 0f);
        
        // Configure velocity over lifetime for drift and variation
        var velocityOverLifetime = waterParticles.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.2f, 0.2f);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-Mathf.Abs(fallSpeed) * (1f - speedVariation), -Mathf.Abs(fallSpeed) * (1f + speedVariation));
        velocityOverLifetime.z = 0f;
        velocityOverLifetime.speedModifier = new ParticleSystem.MinMaxCurve(0.8f, 1.2f);
        
        // Size over lifetime - drops elongate slightly as they fall
        var sizeOverLifetime = waterParticles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.separateAxes = true;
        sizeOverLifetime.x = new ParticleSystem.MinMaxCurve(1f);
        sizeOverLifetime.y = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(0.3f, 1.1f),
            new Keyframe(1f, 1.2f)
        ));
        
        // Configure color over lifetime - keep fully visible
        var colorOverLifetime = waterParticles.colorOverLifetime;
        colorOverLifetime.enabled = false; // Disable for always visible particles
        
        // Set renderer
        var renderer = waterParticles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortingOrder = 100;
        
        // Create material with stretched texture
        Material particleMat = new Material(Shader.Find("Unlit/Transparent"));
        particleMat.color = Color.white; // Use white with texture
        renderer.material = particleMat;
        
        // Create and assign droplet texture
        Texture2D particleTexture = CreateDropletTexture(64);
        particleMat.mainTexture = particleTexture;
    }
    
    void SpawnInitialDrops()
    {
        // Spawn more particles to make sure they're visible
        int particlesToSpawn = Mathf.Min(maxParticles / 2, 100);
        
        Vector3 roomCenter = transform.position;
        float halfWidth = roomWidth / 2f;
        float halfHeight = roomHeight / 2f;
        
        // Determine spawn area
        float spawnWidth = roomWidth;
        float spawnHeight = roomHeight;
        Vector3 spawnCenter = roomCenter;
        
        if (mainCamera != null)
        {
            float cameraHeight = mainCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * mainCamera.aspect;
            spawnWidth = Mathf.Max(cameraWidth + 4f, roomWidth);
            spawnHeight = Mathf.Max(cameraHeight + 4f, roomHeight);
            spawnCenter = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, roomCenter.z);
        }
        
        float halfSpawnWidth = spawnWidth / 2f;
        float halfSpawnHeight = spawnHeight / 2f;
        
        for (int i = 0; i < particlesToSpawn; i++)
        {
            // Random position throughout the spawn area
            float x = Random.Range(spawnCenter.x - halfSpawnWidth * 0.8f, spawnCenter.x + halfSpawnWidth * 0.8f);
            float y = Random.Range(spawnCenter.y - halfSpawnHeight, spawnCenter.y + halfSpawnHeight);
            
            // Create particle with variation
            float speedMod = Random.Range(1f - speedVariation, 1f + speedVariation);
            particles[i].position = new Vector3(x, y, spawnCenter.z);
            particles[i].velocity = new Vector3(Random.Range(-0.2f, 0.2f), -fallSpeed * speedMod, 0f);
            particles[i].startLifetime = 15f;
            particles[i].remainingLifetime = Random.Range(5f, 15f);
            
            // Size with stretch
            float baseSize = Random.Range(minParticleSize, maxParticleSize);
            particles[i].startSize3D = new Vector3(baseSize, baseSize * dropStretch, baseSize);
            
            particles[i].rotation = 0f;
            particles[i].angularVelocity = 0f;
            
            // Color with fade-in
            Color startColor = Color.white;
            startColor.a = Random.Range(0.4f, 0.6f);
            particles[i].startColor = startColor;
        }
        
        // Apply particles to the system
        waterParticles.SetParticles(particles, particlesToSpawn);
        waterParticles.Play();
    }
    
    // Create an elongated droplet texture
    Texture2D CreateDropletTexture(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radiusX = size / 2f;
        float radiusY = size / 2.5f; // Slightly elongated
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x - center.x) / radiusX;
                float dy = (y - center.y) / radiusY;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                
                float alpha = 1f - Mathf.Clamp01(distance);
                alpha = Mathf.Pow(alpha, 1.5f); // Softer falloff
                
                // Add slight brightness gradient (brighter in center)
                float brightness = 0.9f + (alpha * 0.1f);
                pixels[y * size + x] = new Color(brightness, brightness, brightness, alpha);
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        
        return texture;
    }
    
    void LateUpdate()
    {
        if (waterParticles == null) return;
        
        // Update parameters in real-time if changed in inspector
        var main = waterParticles.main;
        main.startSpeedMultiplier = fallSpeed;
        main.startColor = waterColor;
        main.startSizeX = new ParticleSystem.MinMaxCurve(minParticleSize, maxParticleSize);
        main.startSizeY = new ParticleSystem.MinMaxCurve(minParticleSize * dropStretch, maxParticleSize * dropStretch);
        
        var velocityOverLifetime = waterParticles.velocityOverLifetime;
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-Mathf.Abs(fallSpeed) * (1f - speedVariation), -Mathf.Abs(fallSpeed) * (1f + speedVariation));
        
        var shape = waterParticles.shape;
        shape.scale = new Vector3(roomWidth * 0.8f, 1f, 1f);
        
        // Update position based on room height
        if (waterParticles.transform.localPosition.y != roomHeight / 2f + 0.5f)
        {
            waterParticles.transform.localPosition = new Vector3(0f, roomHeight / 2f + 0.5f, 0f);
        }
        
        // Destroy particles outside camera bounds
        if (mainCamera != null)
        {
            DestroyParticlesOutsideCamera();
        }
    }
    
    void DestroyParticlesOutsideCamera()
    {
        // Get camera bounds in world space
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        
        Vector3 camPos = mainCamera.transform.position;
        
        float minX = camPos.x - (cameraWidth / 2f) - cameraBoundsMargin;
        float maxX = camPos.x + (cameraWidth / 2f) + cameraBoundsMargin;
        float minY = camPos.y - (cameraHeight / 2f) - cameraBoundsMargin;
        float maxY = camPos.y + (cameraHeight / 2f) + cameraBoundsMargin;
        
        // Get all particles
        int numParticlesAlive = waterParticles.GetParticles(particles);
        int aliveCount = 0;
        
        // Filter out particles outside camera bounds
        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 pos = particles[i].position;
            
            // Keep particle if it's within camera bounds
            if (pos.x >= minX && pos.x <= maxX && pos.y >= minY && pos.y <= maxY)
            {
                particles[aliveCount] = particles[i];
                aliveCount++;
            }
        }
        
        // Apply filtered particles back to system
        waterParticles.SetParticles(particles, aliveCount);
    }
    
    // Visualize the water drop emitter area and camera bounds in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.6f, 0.7f, 0.8f, 0.5f);
        Vector3 topCenter = transform.position + new Vector3(0f, roomHeight / 2f + 0.5f, 0f);
        Gizmos.DrawWireCube(topCenter, new Vector3(roomWidth * 0.8f, 0.5f, 1f));
        
        // Draw room bounds
        Gizmos.color = new Color(0.5f, 0.6f, 0.7f, 0.2f);
        Gizmos.DrawWireCube(transform.position, new Vector3(roomWidth, roomHeight, 1f));
        
        // Draw camera bounds (if camera is assigned)
        if (mainCamera != null)
        {
            Gizmos.color = new Color(0.4f, 0.6f, 0.8f, 0.4f);
            float cameraHeight = mainCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * mainCamera.aspect;
            
            Vector3 camPos = mainCamera.transform.position;
            Vector3 boundsSize = new Vector3(
                cameraWidth + (cameraBoundsMargin * 2f), 
                cameraHeight + (cameraBoundsMargin * 2f), 
                1f
            );
            
            Gizmos.DrawWireCube(new Vector3(camPos.x, camPos.y, transform.position.z), boundsSize);
        }
    }
}