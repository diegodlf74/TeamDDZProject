using UnityEngine;

public class HealPowerUp : MonoBehaviour
{
    public int healAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null && playerHealth.currentHealth < playerHealth.maxHealth)
        {
            playerHealth.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}