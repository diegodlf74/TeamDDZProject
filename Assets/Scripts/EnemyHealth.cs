using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Level Exit")]
    public EnemyTracker enemyTracker;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

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