using System.Collections;
using System.Collections.Generic;
using MOVEMENT;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    public LayerMask layerMask;
    private Collider2D col;
    private Collider2D playerOnPlatform;
    private PlayerController player;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & layerMask) != 0)
        {
            player = other.gameObject.GetComponent<PlayerController>();
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {

        if (player != null && player.isPressingDown)
        {
            // Start fall through
            StartCoroutine(DisableCollisionTemporarily(other.collider, player));
        }
    }
    // private void OnCollisionStay2D(Collision2D other)
    // {
    //     if (player == null)
    //         return;

    //     if (player.isPressingDown)
    //         Physics2D.IgnoreCollision(playerOnPlatform, col, true);

    // }

    // private void OnCollisionExit2D(Collision2D other)
    // {

    //     if (((1 << other.gameObject.layer) & layerMask) != 0 && !player.isPressingDown)
    //     {
    //         Debug.Log("eit");
    //         Physics2D.IgnoreCollision(other.collider, col, false);
    //         playerOnPlatform = null;
    //         player = null;
    //     }
    // }

    private IEnumerator DisableCollisionTemporarily(Collider2D playerCol, PlayerController player)
    {
        // Disable collision
        Physics2D.IgnoreCollision(playerCol, col, true);

        // Wait until player is fully below platform
        float timer = 0.25f; // quarter-second window
        while (timer > 0f && player.isPressingDown)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // Re-enable collision
        Physics2D.IgnoreCollision(playerCol, col, false);
    }
}
