using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Health")]
    public int maxHealth = 20;
    private int currentHealth;

    [Header("Boss Objects")]
    public int objectsRequired = 3;
    private int objectsDestroyed = 0;

    [Header("Shield")]
    public GameObject shieldVisual;

    [Header("Explosion Attack")]
    public GameObject explosionPrefab;
    public Transform player;

    public float explosionAttackDelay = 1f;
    public float explosionAttackCooldown = 3f;

    [Header("Explosion Damage")]
    public int weakExplosionDamage = 1;
    public int strongExplosionDamage = 3;

    private bool shieldActive = true;
    private bool dead = false;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (shieldVisual != null)
            shieldVisual.SetActive(true);
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            if (p != null)
                player = p.transform;
        }

        InvokeRepeating(
            nameof(CreateExplosionAtPlayer),
            explosionAttackDelay,
            explosionAttackCooldown
        );
    }

    public void ObjectDestroyed()
    {
        objectsDestroyed++;

        Debug.Log(
            "Boss object destroyed: " +
            objectsDestroyed +
            "/" +
            objectsRequired
        );

        if (objectsDestroyed >= objectsRequired)
        {
            shieldActive = false;

            Debug.Log("Boss shield is down!");

            if (shieldVisual != null)
                shieldVisual.SetActive(false);
        }
    }

    private void CreateExplosionAtPlayer()
    {
        if (dead) return;
        if (player == null) return;
        if (explosionPrefab == null) return;

        Vector3 spawnPosition = player.position;

        GameObject explosion = Instantiate(
            explosionPrefab,
            spawnPosition,
            Quaternion.identity
        );

        ExplosionDamage damageScript =
            explosion.GetComponent<ExplosionDamage>();

        if (damageScript != null)
        {
            if (shieldActive)
            {
                damageScript.damage = weakExplosionDamage;
            }
            else
            {
                damageScript.damage = strongExplosionDamage;
            }
        }

        Debug.Log("Boss spawned explosion!");
    }

    public void TakeDamage(int damage)
    {
        if (dead) return;

        if (shieldActive)
        {
            Debug.Log("Boss shield blocked damage!");
            return;
        }

        currentHealth -= damage;

        Debug.Log(
            "Boss took damage. HP: " +
            currentHealth +
            "/" +
            maxHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        dead = true;

        CancelInvoke(nameof(CreateExplosionAtPlayer));

        Debug.Log("Boss defeated!");

        Destroy(gameObject);
    }
}