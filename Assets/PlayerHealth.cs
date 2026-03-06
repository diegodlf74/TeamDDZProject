using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("Player died!");
            // You can reload scene, respawn, etc.

            Destroy(gameObject);
        }
    }
}