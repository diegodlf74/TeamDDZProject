using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Sound Effects")]
    public AudioSource sfxSource;
    public AudioClip takeDamageSound;

    public GameOverScript gameOverScript;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (takeDamageSound != null && sfxSource != null)
            sfxSource.PlayOneShot(takeDamageSound);

        Debug.Log($"Player took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("Player died!");

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