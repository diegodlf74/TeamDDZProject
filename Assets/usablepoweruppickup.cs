using UnityEngine;

public class UsablePowerUpPickup : MonoBehaviour
{
    public UsablePowerUpType powerUpType = UsablePowerUpType.Invisibility;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.GiveUsablePowerUp(powerUpType);
            Destroy(gameObject);
        }
    }
}