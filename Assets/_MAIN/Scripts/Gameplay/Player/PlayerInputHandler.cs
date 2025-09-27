using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private InputReader input;

    public event Action<Vector2> OnMove;
    public event Action OnJumpPressed;
    public event Action OnJumpHeld;
    public event Action OnJumpReleased;
    public event Action OnDash;
    public event Action OnInteract;

    private void OnEnable()
    {
        input.MoveEvent += HandleMove;
        input.JumpPressed += HandleJumpPressed;
        input.JumpEvent += HandleJumpHeld;
        input.JumpCancelledEvent += HandleJumpReleased;
        input.DashEvent += HandleDash;
        input.InteractEvent += HandleInteract;
    }

    private void OnDisable()
    {
        input.MoveEvent -= HandleMove;
        input.JumpPressed -= HandleJumpPressed;
        input.JumpEvent -= HandleJumpHeld;
        input.JumpCancelledEvent -= HandleJumpReleased;
        input.DashEvent -= HandleDash;
        input.InteractEvent -= HandleInteract;
    }

    private void HandleMove(Vector2 dir)
    {
        OnMove?.Invoke(dir);
    }
    private void HandleJumpPressed() => OnJumpPressed?.Invoke();
    private void HandleJumpHeld() => OnJumpHeld?.Invoke();
    private void HandleJumpReleased() => OnJumpReleased?.Invoke();
    private void HandleDash() => OnDash?.Invoke();
    private void HandleInteract() => OnInteract?.Invoke();
}
