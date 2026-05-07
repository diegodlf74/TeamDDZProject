using UnityEngine;
using System.Collections;

public class BossObject : MonoBehaviour
{
    [Header("Health")]
    public int health = 3;

    [Header("Boss Reference")]
    public BossHealth bossHealth;

    [Header("Explosion")]
    public GameObject explosionPrefab;
    public float explosionDelay = 1.5f;
    public float explosionRadius = 3f;
    public int explosionDamage = 1;
    public LayerMask playerLayer;

    private bool exploding = false;

    public void TakeDamage(int damage)
    {
        if (exploding) return;

        health -= damage;

        Debug.Log(name + " took damage. HP: " + health);

        if (health <= 0)
        {
            StartCoroutine(DelayedExplosion());
        }
    }

    private IEnumerator DelayedExplosion()
    {
        exploding = true;

        Debug.Log(name + " is about to explode!");

        yield return new WaitForSeconds(explosionDelay);

        // Spawn explosion effect
        if (explosionPrefab != null)
        {
            Instantiate(
                explosionPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        // Damage player
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            playerLayer
        );

        foreach (Collider hit in hits)
        {
            PlayerHealth playerHealth =
                hit.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(explosionDamage);
                break;
            }
        }

        // Notify boss
        if (bossHealth != null)
        {
            bossHealth.ObjectDestroyed();
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}