using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    private float movementX;
    private float movementY;

    public float speed = 5f;

    private List<SpeedModifier> speedModifiers = new List<SpeedModifier>();

    public float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;

    [Header("Sound Effects")]
    public AudioSource sfxSource;
    public AudioClip attackSound;
    public AudioClip hitConnectSound;

    private Animator anim;

    [Header("Attack")]
    public float attackRadius = 0.6f;
    public int attackDamage = 1;
    public LayerMask enemyLayers;
    public Transform attackPoint;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;

        if (attackSound != null && sfxSource != null)
            sfxSource.PlayOneShot(attackSound);

        anim.SetTrigger("Attack");

        Invoke(nameof(DoAttackHit), 0.1f);
    }

    private float GetFinalSpeed()
    {
        float finalSpeed = speed;

        foreach (SpeedModifier mod in speedModifiers)
        {
            finalSpeed *= mod.multiplier;
        }

        return finalSpeed;
    }

    public void AddSpeedModifier(float multiplier, float duration)
    {
        SpeedModifier mod = new SpeedModifier(multiplier, duration);
        speedModifiers.Add(mod);

        StartCoroutine(RemoveModifier(mod));
    }

    private IEnumerator RemoveModifier(SpeedModifier mod)
    {
        yield return new WaitForSeconds(mod.duration);
        speedModifiers.Remove(mod);
    }

    private void FixedUpdate()
    {
        Vector3 move = new Vector3(movementX, 0f, movementY);
        bool moving = move.sqrMagnitude > 0.01f;

        rb.angularVelocity = Vector3.zero;

        float finalSpeed = GetFinalSpeed();

        if (moving)
        {
            move = move.normalized;

            rb.linearVelocity = new Vector3(
                move.x * finalSpeed,
                rb.linearVelocity.y,
                move.z * finalSpeed
            );

            Quaternion targetRot = Quaternion.LookRotation(move);

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    targetRot,
                    10f * Time.fixedDeltaTime
                )
            );
        }
        else
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }

        anim.SetBool("isWalking", moving);
    }

    private void DoAttackHit()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("No attackPoint assigned on PlayerController.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(
            attackPoint.position,
            attackRadius,
            enemyLayers
        );

        HashSet<BossObject> damagedBossObjects = new HashSet<BossObject>();
        HashSet<BossHealth> damagedBosses = new HashSet<BossHealth>();
        HashSet<EnemyHealth> damagedEnemies = new HashSet<EnemyHealth>();

        bool hitEnemy = false;

        foreach (Collider hit in hits)
        {
            BossObject bossObject = hit.GetComponentInParent<BossObject>();
            if (bossObject != null && !damagedBossObjects.Contains(bossObject))
            {
                bossObject.TakeDamage(attackDamage);
                damagedBossObjects.Add(bossObject);
                hitEnemy = true;
            }

            BossHealth bossHealth = hit.GetComponentInParent<BossHealth>();
            if (bossHealth != null && !damagedBosses.Contains(bossHealth))
            {
                bossHealth.TakeDamage(attackDamage);
                damagedBosses.Add(bossHealth);
                hitEnemy = true;
            }

            EnemyHealth enemyHealth = hit.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null && !damagedEnemies.Contains(enemyHealth))
            {
                enemyHealth.TakeDamage(attackDamage);
                damagedEnemies.Add(enemyHealth);
                hitEnemy = true;
            }
        }

        if (hitEnemy && hitConnectSound != null && sfxSource != null)
            sfxSource.PlayOneShot(hitConnectSound);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
