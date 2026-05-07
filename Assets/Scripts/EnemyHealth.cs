using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Sound Effects")]
    public AudioSource sfxSource;
    public AudioClip takeDamageSound;

    [Header("Level Exit")]
    public EnemyTracker enemyTracker;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (takeDamageSound != null && sfxSource != null)
            sfxSource.PlayOneShot(takeDamageSound);

        Debug.Log($"{name} took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (enemyTracker != null)
        {
            enemyTracker.EnemyDied();
        }

        Destroy(gameObject);
    }
}
