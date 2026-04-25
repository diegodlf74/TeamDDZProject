using UnityEngine;

public class BossObject : MonoBehaviour
{
    public int health = 3;
    public BossHealth bossHealth;

    private bool destroyed = false;

    public void TakeDamage(int damage)
    {
        if (destroyed) return;

        health -= damage;
        Debug.Log(name + " took damage. HP: " + health);

        if (health <= 0)
        {
            destroyed = true;

            if (bossHealth != null)
            {
                bossHealth.ObjectDestroyed();
            }
            else
            {
                Debug.LogWarning(name + " has no BossHealth assigned!");
            }

            Destroy(gameObject);
        }
    }
}