using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;

    [Header("Patrol")]
    public Transform[] waypoints;
    public float waypointReachDistance = 0.3f;
    public float patrolStoppingDistance = 0f;
    public float patrolSpeed = 2.5f;

    [Header("Attack")]
    public float attackRange = 0.8f;
    public float attackCooldown = 1.5f;
    public float attackDuration = 1.0f;
    public float chaseSpeed = 3.5f;

    [Header("Damage")]
    public int damage = 1;
    public float hitRadius = 1f;
    public Transform hitPoint;

    private NavMeshAgent agent;
    private Animator animator;

    private float nextAttackTime;
    private bool isAttacking;
    private bool isChasing;
    private bool hasDealtDamageThisAttack;

    private int currentWaypointIndex;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        isChasing = false;
        currentWaypointIndex = 0;

        agent.stoppingDistance = attackRange;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        if (!agent.enabled)
        {
            agent.enabled = true;
        }

        if (agent.enabled && agent.isOnNavMesh && waypoints != null && waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Update()
    {
        if (player == null) return;
        if (!agent.enabled || !agent.isOnNavMesh) return;

        if (isChasing)
        {
            HandleChase();
        }
        else
        {
            HandlePatrol();
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (isChasing && !isAttacking)
        {
            if (agent.pathPending) return;

            bool closeEnough = agent.remainingDistance <= agent.stoppingDistance + 0.1f;

            if (closeEnough && Time.time >= nextAttackTime)
            {
                StartJumpAttack();
            }
        }

        if (isAttacking)
        {
            HandleAttackDamage();
        }
    }

    void HandlePatrol()
    {
        if (isAttacking) return;

        if (waypoints == null || waypoints.Length == 0)
        {
            agent.isStopped = true;
            return;
        }

        agent.speed = patrolSpeed;
        agent.stoppingDistance = patrolStoppingDistance;
        agent.isStopped = false;

        if (!agent.pathPending && agent.remainingDistance <= waypointReachDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void HandleChase()
    {
        if (isAttacking) return;

        agent.speed = chaseSpeed;
        agent.stoppingDistance = attackRange;
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void StartJumpAttack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;
        hasDealtDamageThisAttack = false;

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir);

        animator.Play("JumpStart", 0, 0f);

        CancelInvoke(nameof(EndAttack));
        Invoke(nameof(EndAttack), attackDuration);
    }

    void EndAttack()
    {
        isAttacking = false;

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;

            if (isChasing)
            {
                agent.SetDestination(player.position);
            }
            else if (waypoints != null && waypoints.Length > 0)
            {
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
        }
    }

    void HandleAttackDamage()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("JumpStart") && !hasDealtDamageThisAttack && stateInfo.normalizedTime >= 0.8f)
        {
            DoEnemyHit();
        }
    }

    public void DoEnemyHit()
    {
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
                break;
            }
        }
    }

    public void EnableChase()
    {
        isChasing = true;

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
            agent.isStopped = false;

            if (waypoints != null && waypoints.Length > 0)
            {
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
        }
    }

    public void OnLand()
    {
    }

    public void OnFootstep()
    {
    }
}