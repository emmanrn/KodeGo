using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 1;
    private int currentHealth;
    public HealthUI healthUI;
    public static event Action OnPlayerDied;
    void Start()
    {
        currentHealth = maxHealth;
        healthUI.setMaxHearts(maxHealth);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy)
        {
            TakeDamage();
        }
    }
    private void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            
        }
    }
}
