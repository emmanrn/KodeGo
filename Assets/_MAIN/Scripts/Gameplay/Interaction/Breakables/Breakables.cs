using MOVEMENT;
using UnityEngine;

public class Breakables : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayerController playerController = other.collider.GetComponent<PlayerController>();
        if (playerController != null)
            this.gameObject.SetActive(false);
    }

}
