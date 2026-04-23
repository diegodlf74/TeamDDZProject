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
    private bool isChasing;

    // IMPORTANT: prevents instant multi-hit
    private bool hasDealtDamageThisAttack;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        isChasing = false;

        agent.enabled = false; // 👈 start disabled

        agent.stoppingDistance = attackRange;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        if (!isChasing)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        if (!isAttacking && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }

        animator.SetFloat("Speed", (agent.enabled && agent.isOnNavMesh) ? agent.velocity.magnitude : 0f);

        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        if (agent.pathPending)
            return;

        bool closeEnough = agent.remainingDistance <= agent.stoppingDistance + 0.1f;

        if (!isAttacking && closeEnough && Time.time >= nextAttackTime)
        {
            StartJumpAttack();
        }

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

    public void EnableChase()
    {
        isChasing = true;

        if (!agent.enabled)
        {
            agent.enabled = true;
        }

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    public void DisableChase()
    {
        isChasing = false;

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    public void OnLand()
    {
        // Exists only so the animation event has a receiver.
    }

    public void OnFootstep()
    {

    }

}