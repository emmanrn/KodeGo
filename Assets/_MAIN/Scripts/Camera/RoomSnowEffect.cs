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
        public float roomWidth = 25f;
        public float roomHeight = 15f;
        
        [Header("Particle Settings")]
        public float emissionRate = 150f;
        public int maxParticles = 5000;
        [Range(0.01f, 0.3f)]
        public float minParticleSize = 0.08f;
        [Range(0.1f, 0.5f)]
        public float maxParticleSize = 0.2f;
        
        [Header("Camera Bounds")]
        [Tooltip("Particles outside camera view will be destroyed")]
        public Camera mainCamera;
        [Tooltip("Extra margin beyond camera bounds (in world units)")]
        public float cameraBoundsMargin = 2f;
        
        private ParticleSystem snowParticles;
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
            
            CreateSnowParticles();
            SpawnInitialSnow();
        }
        
        void CreateSnowParticles()
        {
            // Create particle system as child of room
            GameObject psObj = new GameObject("SnowParticles");
            psObj.transform.SetParent(transform);
            
            // Position at the top of the room
            psObj.transform.localPosition = new Vector3(0f, roomHeight / 2f + 1f, 0f);
            
            snowParticles = psObj.AddComponent<ParticleSystem>();
            
            // Initialize particles array
            particles = new ParticleSystem.Particle[maxParticles];
            
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
            main.loop = true;
            main.prewarm = false; // Disabled - we'll spawn manually
            
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
            
            // Configure color over lifetime - keep particles fully visible
            var colorOverLifetime = snowParticles.colorOverLifetime;
            colorOverLifetime.enabled = false; // Disable fading for instant visibility
            
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
        
        void SpawnInitialSnow()
        {
            // Spawn maximum particles to completely fill the room
            int particlesToSpawn = maxParticles;
            
            Vector3 roomCenter = transform.position;
            float halfWidth = roomWidth / 2f;
            float halfHeight = roomHeight / 2f;
            
            // If camera exists, prioritize filling camera view
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
                float x = Random.Range(spawnCenter.x - halfSpawnWidth, spawnCenter.x + halfSpawnWidth);
                float y = Random.Range(spawnCenter.y - halfSpawnHeight, spawnCenter.y + halfSpawnHeight);
                
                // Create particle
                particles[i].position = new Vector3(x, y, spawnCenter.z);
                particles[i].velocity = new Vector3(horizontalVelocity, -fallSpeed, 0f);
                particles[i].startLifetime = 20f;
                particles[i].remainingLifetime = Random.Range(10f, 20f); // Longer lifetimes
                particles[i].startSize = Random.Range(minParticleSize, maxParticleSize);
                particles[i].rotation = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                particles[i].angularVelocity = Random.Range(-90f, 90f) * Mathf.Deg2Rad;
                
                // Full opacity - no fading
                particles[i].startColor = snowColor;
            }
            
            // Apply particles to the system immediately
            snowParticles.SetParticles(particles, particlesToSpawn);
            
            // Force the particle system to update immediately
            snowParticles.Play();
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
            int numParticlesAlive = snowParticles.GetParticles(particles);
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
            snowParticles.SetParticles(particles, aliveCount);
        }
        
        // Visualize the snow emitter area and camera bounds in the Scene view
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
            Vector3 topCenter = transform.position + new Vector3(0f, roomHeight / 2f + 1f, 0f);
            Gizmos.DrawWireCube(topCenter, new Vector3(roomWidth, 0.5f, 1f));
            
            // Draw room bounds
            Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.2f);
            Gizmos.DrawWireCube(transform.position, new Vector3(roomWidth, roomHeight, 1f));
            
            // Draw camera bounds (if camera is assigned)
            if (mainCamera != null)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
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