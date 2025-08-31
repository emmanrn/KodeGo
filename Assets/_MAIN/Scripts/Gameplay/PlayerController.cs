using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    private DIALOGUE.PlayerInputManager playerInputManager => DIALOGUE.PlayerInputManager.instance;
    public float moveSpeed = 5f;
    public float jumpPower = 5f;

    float horizontalMovement;

    void Update()
    {
        rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
    }

    private void OnEnable()
    {
        if (playerInputManager != null)
            playerInputManager.EnablePlayerMovement();
    }

    public void Move(InputAction.CallbackContext c)
    {
        Debug.Log("Move");
        horizontalMovement = c.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext c)
    {
        if (c.performed)
        {
            rb.velocity = new Vector2(rb.velocity.y, jumpPower);
        }
    }
}
