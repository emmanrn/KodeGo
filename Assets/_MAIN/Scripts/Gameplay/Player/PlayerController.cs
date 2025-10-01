using UnityEngine;
using Cinemachine;
using System;
using System.Data.Common;

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

        private Rigidbody2D rb;
        private GroundChecker groundChecker;

        private PlayerMovement movement;
        private PlayerJump jump;
        private PlayerDash dash;
        private PlayerWall wallJump;
        private PlayerInteraction interaction;
        private PlayerAnimation anim;
        private PlayerRespawn respawn;
        private PlayerLife life;
        private PlayerToggleHistoryLogs toggleHistory;

        private bool grounded;
        private bool dashLocked;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            groundChecker = GetComponent<GroundChecker>();

            movement = GetComponent<PlayerMovement>();
            jump = GetComponent<PlayerJump>();
            dash = GetComponent<PlayerDash>();
            wallJump = GetComponent<PlayerWall>();
            interaction = GetComponent<PlayerInteraction>();
            anim = GetComponent<PlayerAnimation>();
            respawn = GetComponent<PlayerRespawn>();
            life = GetComponent<PlayerLife>();
            toggleHistory = GetComponent<PlayerToggleHistoryLogs>();

        }

        private void Start()
        {
            movement.Initialize(rb);
            jump.Initialize(rb, wallJump);
            dash.Initialize(rb, movement);
            wallJump.Initialize(rb);
            anim.Initialize(rb);
            respawn.Initialize(rb, startingPoint, circleTransition, roomCamera);
            life.Initialize(respawn);
        }

        private void OnEnable()
        {
            input.SetPlayerMovement();

            input.MoveEvent += movement.SetMoveDirection;
            input.JumpPressed += jump.OnJumpPressed;
            input.JumpEvent += jump.OnJumpHeld;
            input.JumpCancelledEvent += jump.OnJumpReleased;
            input.DashEvent += dash.OnDash;
            input.InteractEvent += interaction.OnInteract;
            input.ToggleHistoryLogsEvent += toggleHistory.OnToggleHistoryLog;

        }

        private void OnDisable()
        {
            input.MoveEvent -= movement.SetMoveDirection;
            input.JumpPressed -= jump.OnJumpPressed;
            input.JumpEvent -= jump.OnJumpHeld;
            input.JumpCancelledEvent -= jump.OnJumpReleased;
            input.DashEvent -= dash.OnDash;
            input.InteractEvent -= interaction.OnInteract;
            input.ToggleHistoryLogsEvent -= toggleHistory.OnToggleHistoryLog;

        }

        private void Update()
        {
            grounded = groundChecker.isGrounded();
            dashLocked = dash.IsDashing || Time.time < dash.PostDashLockUntil || dash.PostDashIgnoreFramesCounter > 0;

            dash.Tick(grounded);
            wallJump.Tick(movement.MoveDirection, grounded);

            if (dashLocked)
            {
                jump.ClearJumpBuffer();
            }

            if (!dashLocked && jump.IsJumpBuffered)
            {
                // If airborne and wall jump is possible, then trigger a wall jump.
                if (!grounded && wallJump.CanWallJump)
                {
                    wallJump.TriggerWallJump();
                    movement.isFacingRight = !movement.isFacingRight;
                    anim.Jump();
                    return;
                }
                jump.Tick(grounded);
                anim.Jump();
            }


            anim.Tick();


        }

        private void FixedUpdate()
        {
            dash.FixedTick();

            if (!wallJump.IsWallJumping)
                movement.FixedTick(grounded);

            if (!grounded)
                jump.FixedTick();
        }

    }
}
