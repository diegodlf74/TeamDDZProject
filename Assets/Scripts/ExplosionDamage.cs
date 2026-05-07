using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    public float radius = 2.5f;
    public int damage = 1;
    public float lifetime = 1f;

    public LayerMask playerLayer;

    private void Start()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            radius,
            playerLayer
        );

        foreach (Collider hit in hits)
        {
            PlayerHealth playerHealth =
                hit.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                break;
            }
        }

        Destroy(gameObject, lifetime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            radius
        );
    }
}