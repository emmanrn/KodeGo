using UnityEngine;

// Attach this script to each Room GameObject
public class RoomSnowEffect : MonoBehaviour
{
    [Header("Snow Customization")]
    [Range(-2f, 2f)]
    public float horizontalVelocity = -0.5f; // Negative = left, Positive = right
    [Range(0.5f, 5f)]
    public float fallSpeed = 2f;
    public Color snowColor = new Color(0.8f, 0.8f, 0.8f, 1f); // Grey color
    
    [Header("Room Size")]
    public float roomWidth = 20f;
    public float roomHeight = 15f;
    
    [Header("Particle Settings")]
    public float emissionRate = 150f;
    public int maxParticles = 5000;
    [Range(0.01f, 0.3f)]
    public float minParticleSize = 0.08f;
    [Range(0.1f, 0.5f)]
    public float maxParticleSize = 0.2f;
    
    private ParticleSystem snowParticles;
    private ParticleSystem.Particle[] particles;
    
    void Start()
    {
        CreateSnowParticles();
    }
    
    void CreateSnowParticles()
    {
        // Create particle system as child of room
        GameObject psObj = new GameObject("SnowParticles");
        psObj.transform.SetParent(transform);
        
        // Position at the top of the room
        psObj.transform.localPosition = new Vector3(0f, roomHeight / 2f + 1f, 0f);
        
        snowParticles = psObj.AddComponent<ParticleSystem>();
        
        // Configure main module
        var main = snowParticles.main;
        main.startLifetime = 20f; // Long lifetime to cover room height
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startSpeedMultiplier = fallSpeed;
        main.startSize = new ParticleSystem.MinMaxCurve(minParticleSize, maxParticleSize);
        main.startColor = snowColor;
        main.gravityModifier = 0.2f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = maxParticles;
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
        main.loop = true; // Ensure looping
        main.prewarm = true; // Start with particles already spawned
        
        // Configure emission
        var emission = snowParticles.emission;
        emission.rateOverTime = emissionRate;
        
        // Configure shape - Rectangle spanning room width
        var shape = snowParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Rectangle;
        shape.scale = new Vector3(roomWidth, 1f, 1f);
        shape.rotation = new Vector3(90f + (Mathf.Atan2(-fallSpeed, horizontalVelocity) * Mathf.Rad2Deg), 0f, 0f);
        
        // Configure velocity over lifetime for diagonal movement
        var velocityOverLifetime = snowParticles.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(horizontalVelocity);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-Mathf.Abs(fallSpeed));
        velocityOverLifetime.z = 0f;
        
        // Configure rotation over lifetime for gentle spinning
        var rotationOverLifetime = snowParticles.rotationOverLifetime;
        rotationOverLifetime.enabled = true;
        rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(-90f * Mathf.Deg2Rad, 90f * Mathf.Deg2Rad);
        
        // Configure color over lifetime for fade in/out
        var colorOverLifetime = snowParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(1f, 0.05f),
                new GradientAlphaKey(1f, 0.9f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = gradient;
        
        // Set renderer
        var renderer = snowParticles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortingOrder = 100;
        
        // Create material with circle texture
        Material particleMat = new Material(Shader.Find("Unlit/Transparent"));
        particleMat.color = snowColor;
        renderer.material = particleMat;
        
        // Create and assign circle texture
        Texture2D particleTexture = CreateCircleTexture(64);
        particleMat.mainTexture = particleTexture;
    }
    
    // Create a simple circle texture for particles
    Texture2D CreateCircleTexture(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = 1f - Mathf.Clamp01(distance / radius);
                alpha = Mathf.Pow(alpha, 2f); // Smooth falloff
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        texture.wrapMode = TextureWrapMode.Clamp;
        
        return texture;
    }
    
    void LateUpdate()
    {
        if (snowParticles == null) return;
        
        // Update parameters in real-time if changed in inspector
        var main = snowParticles.main;
        main.startSpeedMultiplier = fallSpeed;
        main.startColor = snowColor;
        main.startSize = new ParticleSystem.MinMaxCurve(minParticleSize, maxParticleSize);
        
        var velocityOverLifetime = snowParticles.velocityOverLifetime;
        velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(horizontalVelocity);
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(-Mathf.Abs(fallSpeed));
        
        var shape = snowParticles.shape;
        shape.scale = new Vector3(roomWidth, 1f, 1f);
        
        // Update position based on room height
        if (snowParticles.transform.localPosition.y != roomHeight / 2f + 1f)
        {
            snowParticles.transform.localPosition = new Vector3(0f, roomHeight / 2f + 1f, 0f);
        }
        
        // Kill particles that reach the bottom of the room
        // Calculate bottom Y in world space
        float bottomY = transform.position.y - (roomHeight / 2f);
        int numParticlesAlive = snowParticles.GetParticles(particles);
        int killedCount = 0;
        
        for (int i = 0; i < numParticlesAlive; i++)
        {
            if (particles[i].position.y <= bottomY)
            {
                particles[i].remainingLifetime = 0f;
                killedCount++;
            }
        }
        
        snowParticles.SetParticles(particles, numParticlesAlive);
        
        // Debug output (remove after testing)
        if (killedCount > 0)
        {
            Debug.Log($"Killed {killedCount} particles at bottom Y: {bottomY}");
        }
    }
    
    // Visualize the snow emitter area in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
        Vector3 topCenter = transform.position + new Vector3(0f, roomHeight / 2f + 1f, 0f);
        Gizmos.DrawWireCube(topCenter, new Vector3(roomWidth, 0.5f, 1f));
        
        // Draw room bounds
        Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.2f);
        Gizmos.DrawWireCube(transform.position, new Vector3(roomWidth, roomHeight, 1f));
        
        // Draw bottom boundary where particles get killed
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.5f);
        Vector3 bottomCenter = transform.position + new Vector3(0f, -roomHeight / 2f, 0f);
        Gizmos.DrawLine(new Vector3(transform.position.x - roomWidth / 2f, bottomCenter.y, transform.position.z),
                       new Vector3(transform.position.x + roomWidth / 2f, bottomCenter.y, transform.position.z));
    }
}