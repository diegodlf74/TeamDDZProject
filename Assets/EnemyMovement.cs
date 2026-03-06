using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;

    [Header("Attack")]
    public float attackRange = 0.8f;
    public float attackCooldown = 1.5f;
    public float attackDuration = 1.0f; // how long the jump attack takes (seconds)

    [Header("Damage")]
    public int damage = 1;
    public float hitRadius = 1f;
    public Transform hitPoint;

    private NavMeshAgent agent;
    private Animator animator;

    private float nextAttackTime;
    private bool isAttacking;

    // IMPORTANT: prevents instant multi-hit
    private bool hasDealtDamageThisAttack;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        agent.stoppingDistance = attackRange;
    }

    void Update()
    {
        if (player == null) return;

        // Chase when not attacking
        if (!isAttacking)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }

        // Update walk/run blend
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (agent.pathPending) return;

        bool closeEnough = agent.remainingDistance <= agent.stoppingDistance + 0.1f;

        // Start attack
        if (!isAttacking && closeEnough && Time.time >= nextAttackTime)
        {
            StartJumpAttack();
        }

        // If currently attacking, check for the moment to deal damage
        if (isAttacking)
        {
            HandleAttackDamage();
        }
    }

    void StartJumpAttack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        // Reset damage flag so this attack can hit once
        hasDealtDamageThisAttack = false;

        // Stop movement for attack
        agent.isStopped = true;
        agent.ResetPath();

        // Optional: face the player
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir);

        // Play jump animation directly (no trigger needed)
        animator.Play("JumpStart", 0, 0f);

        // End attack after the jump finishes (since clip is read-only)
        CancelInvoke(nameof(EndAttack));
        Invoke(nameof(EndAttack), attackDuration);
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    void HandleAttackDamage()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Deal damage ONCE when JumpStart passes 40% of its timeline
        if (stateInfo.IsName("JumpStart") && !hasDealtDamageThisAttack && stateInfo.normalizedTime >= 0.8f)
        {
            DoEnemyHit();
        }
    }

    public void DoEnemyHit()
    {
        // Safety: never allow more than one hit per attack
        if (hasDealtDamageThisAttack) return;
        hasDealtDamageThisAttack = true;

        if (player == null) return;

        Vector3 point = hitPoint != null
            ? hitPoint.position
            : transform.position + transform.forward * 1f;

        Collider[] hits = Physics.OverlapSphere(point, hitRadius);

        foreach (Collider c in hits)
        {
            PlayerHealth ph = c.GetComponentInParent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
                break; // only hit once
            }
        }
    }

}