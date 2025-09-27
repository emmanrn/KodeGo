using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour, IDamageable
{
    private int maxLife = 3;
    private int currentLife;


    void Start()
    {
        currentLife = maxLife;
        GameManager.instance.RegisterPlayer(this);
    }
    // TODO
    // Make the lives UI update as well btw
    public void TakeDamage(int dmg)
    {
        currentLife -= dmg;
        Debug.Log(currentLife);

        if (currentLife <= 0)
            Debug.Log("Dead Repaawn");
    }



}
