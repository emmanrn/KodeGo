using System;
using System.Collections;
using System.Collections.Generic;
using PLAYER;
using UnityEditor.VersionControl;
using UnityEngine;

public class PlayerLife : MonoBehaviour, IDamageable
{
    private int maxLife = 3;
    private int currentLife;
    private PlayerRespawn respawn;
    public int CurrentLives => currentLife;
    public event Action<int> OnLivesChanged;

    public void Initialize(PlayerRespawn respawn)
    {
        this.respawn = respawn;
    }

    void Start()
    {
        currentLife = maxLife;
        GameManager.instance.RegisterPlayer(this);
        OnLivesChanged?.Invoke(currentLife);
    }
    // TODO
    // Make the lives UI update as well btw
    public void TakeDamage(int dmg)
    {
        currentLife -= dmg;
        Debug.Log(currentLife);

        OnLivesChanged?.Invoke(currentLife);



        if (currentLife <= 0)
        {
            Debug.Log("Dead Repaawn");
            respawn.Die(true);

            currentLife = maxLife;
            OnLivesChanged?.Invoke(currentLife);

            GameEvents.PlayerDied();
        }
    }



}
