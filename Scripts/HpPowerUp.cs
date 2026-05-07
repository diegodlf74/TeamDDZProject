using UnityEngine;

public class HpPowerUp : MonoBehaviour
{
    public int maxHealthIncrease = 1;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.IncreaseMaxHealth(maxHealthIncrease);
            Destroy(gameObject);
        }
    }
}