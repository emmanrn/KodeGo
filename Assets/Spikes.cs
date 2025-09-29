using System.Collections;
using System.Collections.Generic;
using MOVEMENT;
using PLAYER;
using UnityEngine;

public class Spikes : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerRespawn>().Die();
        }
    }
}
