using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    public HealthTextUI healthUI;

    [Header("Sound Effects")]
    public AudioSource sfxSource;
    public AudioClip takeDamageSound;

    public GameOverScript gameOverScript;

    void Awake()
    {
        if (GameManager.Instance != null && GameManager.Instance.playerMaxHP > 0)
        {
            maxHealth = GameManager.Instance.playerMaxHP;
            currentHealth = GameManager.Instance.playerHP;
        }
        else
        {
            currentHealth = maxHealth;
            SaveHealth();
        }

        healthUI.UpdateHealth(currentHealth, maxHealth);
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

            if (GameManager.Instance != null)
            {
                GameManager.Instance.playerHP = 2; // reset for next run
                GameManager.Instance.playerMaxHP = 2;
            }

            SceneManager.LoadScene("MainMenu");
        }

        healthUI.UpdateHealth(currentHealth, maxHealth);
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth; // or keep Mathf.Min if you don't want full heal

        SaveHealth();

        Debug.Log($"Max HP increased by {amount}. Max HP: {maxHealth}, Current HP: {currentHealth}");
        healthUI.UpdateHealth(currentHealth, maxHealth);
    }


    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        SaveHealth();

        Debug.Log($"Player healed {amount}. HP: {currentHealth}");
        healthUI.UpdateHealth(currentHealth, maxHealth);
    }

    private void SaveHealth()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerHP = currentHealth;
            GameManager.Instance.playerMaxHP = maxHealth;
        }
    }
}
