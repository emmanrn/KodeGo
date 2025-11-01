using UnityEngine;
using System.Collections.Generic;

public class ParticleCollisionDestroyer2D : MonoBehaviour
{
    [Header("Collision Settings")]
    [Tooltip("Particles will be destroyed when touching objects on these layers")]
    public LayerMask collisionLayers = -1; // All layers by default
    
    [Header("Effects")]
    [Tooltip("Particle system to spawn on collision (optional)")]
    public ParticleSystem collisionEffect;
    
    [Tooltip("Sound to play on collision (optional)")]
    public AudioClip collisionSound;
    
    private ParticleSystem ps;
    private List<ParticleCollisionEvent> collisionEvents;
    private AudioSource audioSource;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        
        if (ps == null)
        {
            Debug.LogError("ParticleCollisionDestroyer2D requires a ParticleSystem component!");
            enabled = false;
            return;
        }
        
        // Enable collision module
        var collision = ps.collision;
        collision.enabled = true;
        collision.type = ParticleSystemCollisionType.World;
        collision.mode = ParticleSystemCollisionMode.Collision2D;
        collision.sendCollisionMessages = true;
        collision.collidesWith = collisionLayers;
        
        // Setup collision event list
        collisionEvents = new List<ParticleCollisionEvent>();
        
        // Setup audio source for collision sounds
        if (collisionSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = ps.GetCollisionEvents(other, collisionEvents);
        
        // Get all particles
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.main.maxParticles];
        int numParticles = ps.GetParticles(particles);
        
        // Track which particles to remove
        bool[] shouldRemove = new bool[numParticles];
        
        // Mark particles that collided for removal
        for (int i = 0; i < numCollisionEvents; i++)
        {
            var collisionEvent = collisionEvents[i];
            
            // Find the particle that collided
            for (int j = 0; j < numParticles; j++)
            {
                if (Vector3.Distance(particles[j].position, collisionEvent.intersection) < 0.1f)
                {
                    shouldRemove[j] = true;
                    
                    // Spawn collision effect
                    if (collisionEffect != null)
                    {
                        Instantiate(collisionEffect, collisionEvent.intersection, Quaternion.identity);
                    }
                    
                    // Play collision sound
                    if (collisionSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(collisionSound);
                    }
                    
                    break;
                }
            }
        }
        
        // Remove collided particles by copying non-removed particles
        int aliveCount = 0;
        for (int i = 0; i < numParticles; i++)
        {
            if (!shouldRemove[i])
            {
                particles[aliveCount] = particles[i];
                aliveCount++;
            }
        }
        
        // Apply the filtered particles back to the system
        ps.SetParticles(particles, aliveCount);
    }
}