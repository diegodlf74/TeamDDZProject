using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    public GameOverScript gameOverScript;

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

            if (gameOverScript != null)
            {
                gameOverScript.GameOver();
            }
            else
            {
                Debug.LogError("GameOverScript is not assigned on PlayerHealth!");
            }
        }
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;

        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log($"Max HP increased by {amount}. Max HP: {maxHealth}, Current HP: {currentHealth}");
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log($"Player healed {amount}. HP: {currentHealth}");
    }
}