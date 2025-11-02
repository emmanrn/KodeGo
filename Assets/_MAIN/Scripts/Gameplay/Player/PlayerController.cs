using UnityEngine;
using Cinemachine;
using System;
using System.Data.Common;
using System.Collections;

namespace PLAYER
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader input;
        [SerializeField] private Transform startingPoint;
        [SerializeField] private CinemachineVirtualCamera roomCamera;
        [SerializeField] private CircleTransition circleTransition;
        [SerializeField] private Animator anim;

        private Rigidbody2D rb;
        private PlayerMovement movement;
        private PlayerInteraction interaction;
        private CinemachineImpulseSource impulseSource;
        // private PlayerAnimation anim;
        private PlayerRespawn respawn;
        private PlayerLife life;
        private PlayerToggleHistoryLogs toggleHistory;
        private GameObject currentPassThroughPlatform;
        private bool isPassingThroughPlatform;

        [Header("Screen Shake Settings")]
        [SerializeField] private float hardLandVelocityThreshold = 15f;
        [SerializeField] private float screenShakeIntensity = 1.5f;

        [Header("Platform Pass-Through Settings")]
        [SerializeField] private float platformPassThroughResetDelay = 0.2f;

        [Header("Audio")]
        [SerializeField] private AudioClip jumpSound;
        [SerializeField] private AudioClip dashSound;
        [SerializeField] private AudioClip jumpLandSound;
        [SerializeField] private AudioClip hardLandSound;
        [SerializeField] private AudioClip footstepSound;
        [SerializeField] private AudioClip wallSlideSound;
        [SerializeField] private AudioClip wallJumpSound;
        [SerializeField] private AudioClip slideSound;

        private AudioSource audioSource;
        private AudioSource footstepAudioSource;

        [Header("Particle Effects")]
        [SerializeField] private ParticleSystem jumpLandParticles;
        [SerializeField] private ParticleSystem hardLandParticles;
        public bool isDashing => movement.IsDashing;
        private bool wasGrounded;
        private float downPressTime;
        public bool isPressingDown;


        private void Awake()
        {
            CameraManager.SwitchCamera(roomCamera);
            rb = GetComponent<Rigidbody2D>();
            movement = GetComponent<PlayerMovement>();
            interaction = GetComponent<PlayerInteraction>();
            respawn = GetComponent<PlayerRespawn>();
            life = GetComponent<PlayerLife>();
            toggleHistory = GetComponent<PlayerToggleHistoryLogs>();

            // Setup audio sources
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;

            // Create separate audio source for footsteps
            footstepAudioSource = gameObject.AddComponent<AudioSource>();
            footstepAudioSource.playOnAwake = false;
            footstepAudioSource.loop = true;

            // Setup screen shake impulse source
            impulseSource = GetComponent<CinemachineImpulseSource>();
            if (impulseSource == null && roomCamera != null)
            {
                impulseSource = gameObject.AddComponent<CinemachineImpulseSource>();
            }

        }

        private void Start()
        {
            movement.Initialize(rb, anim);
            respawn.Initialize(rb, startingPoint, circleTransition, roomCamera);
            life.Initialize(respawn);
        }

        private void OnEnable()
        {
            input.SetPlayerMovement();
            input.MoveEvent += movement.SetMoveDirection;
            input.JumpPressed += movement.OnJumpInput;
            input.JumpCancelledEvent += movement.OnJumpUpInput;
            input.DashEvent += movement.OnDashInput;
            input.InteractEvent += interaction.OnInteract;
            input.ToggleHistoryLogsEvent += toggleHistory.OnToggleHistoryLog;

        }

        private void OnDisable()
        {
            input.MoveEvent -= movement.SetMoveDirection;
            input.JumpPressed -= movement.OnJumpInput;
            input.JumpCancelledEvent -= movement.OnJumpUpInput;
            input.DashEvent -= movement.OnDashInput;
            input.InteractEvent -= interaction.OnInteract;
            input.ToggleHistoryLogsEvent -= toggleHistory.OnToggleHistoryLog;

        }

        private void Update()
        {
            if (movement._moveInput.y < -0.5f)
                downPressTime += Time.deltaTime;
            else
                downPressTime = 0f;

            isPressingDown = downPressTime > 0.1f; // must hold down for 0.2s
            bool currentlyGrounded = movement.LastOnGroundTime > -0.05f;

            // Handle landing effects with screen shake
            if (currentlyGrounded && !wasGrounded)
            {
                HandleLanding();
                // Reset pass-through when landing on a platform
                ResetPassThrough();
            }

            // Track platform pass-through
            /* The `HandlePlatformPassThrough(currentlyGrounded);` method in the `PlayerController`
            class is responsible for managing the player's ability to pass through platforms. Here's
            a breakdown of what it does: */
            HandlePlatformPassThrough(currentlyGrounded);
            movement.Tick();



            HandleFootsteps(currentlyGrounded);
            wasGrounded = currentlyGrounded;
        }

        private void FixedUpdate()
        {
            movement.FixedTick();
        }

        private void OnJumpPressed()
        {
            movement.OnJumpInput();

            if (jumpSound != null)
                AudioManager.instance.PlaySoundEffect(jumpSound);
        }

        private void OnDash()
        {
            movement.OnDashInput();

            if (dashSound != null)
                AudioManager.instance.PlaySoundEffect(dashSound);
        }

        private void HandleLanding()
        {
            if (jumpLandSound != null)
                AudioManager.instance.PlaySoundEffect(jumpLandSound);

        }

        private void HandleFootsteps(bool isGrounded)
        {
            bool isMoving = movement._moveInput.x != 0 && isGrounded && !movement.IsDashing;

            if (isMoving && footstepSound != null)
            {
                if (!footstepAudioSource.isPlaying)
                {
                    footstepAudioSource.clip = footstepSound;
                    footstepAudioSource.volume = 0;
                    footstepAudioSource.Play();
                    AudioManager.instance.PlaySoundEffect(footstepSound, loop: true);
                }
            }
            else
            {
                if (footstepAudioSource.isPlaying)
                {
                    footstepAudioSource.Stop();
                    AudioManager.instance.StopSoundEffect(footstepSound);
                }
            }
        }

        private void HandlePlatformPassThrough(bool currentlyGrounded)
        {
            // If pressing down and grounded, start pass-through
            if (isPressingDown && currentlyGrounded && !isPassingThroughPlatform)
            {
                // Get the platform we're standing on
                GameObject platform = GetCurrentPlatform();
                if (platform != null)
                {
                    currentPassThroughPlatform = platform;
                    isPassingThroughPlatform = true;
                }
            }

            // Reset when player has cleared the platform
            if (isPassingThroughPlatform && !currentlyGrounded)
            {
                // Check if player has moved below the platform
                if (currentPassThroughPlatform != null)
                {
                    float platformBottom = currentPassThroughPlatform.GetComponent<Collider2D>().bounds.min.y;
                    float playerTop = GetComponent<Collider2D>().bounds.max.y;

                    // If player is completely below the platform, reset
                    if (playerTop < platformBottom - 0.1f)
                    {
                        StartCoroutine(ResetPassThroughAfterDelay());
                    }
                }
            }
        }

        private GameObject GetCurrentPlatform()
        {
            // Cast down to find the platform we're standing on
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Ground"));
            return hit.collider ? hit.collider.gameObject : null;
        }

        private IEnumerator ResetPassThroughAfterDelay()
        {
            yield return new WaitForSeconds(platformPassThroughResetDelay);
            ResetPassThrough();
        }

        private void ResetPassThrough()
        {
            currentPassThroughPlatform = null;
            isPassingThroughPlatform = false;
        }

        // Public method to check if player can pass through a specific platform
        public bool CanPassThroughPlatform(GameObject platform)
        {
            // Can only pass through if pressing down and either:
            // 1. Not currently passing through any platform, OR
            // 2. This is the current pass-through platform
            if (!isPressingDown)
                return false;

            if (!isPassingThroughPlatform)
                return true;

            return platform == currentPassThroughPlatform;
        }
    }
}

