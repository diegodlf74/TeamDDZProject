using UnityEngine;

public class SpeedPowerUp : MonoBehaviour
{
    public float multiplier = 2f;
    public float duration = 5f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.AddSpeedModifier(multiplier, duration);
            Destroy(gameObject);
        }
    }
}
