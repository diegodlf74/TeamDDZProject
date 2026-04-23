using UnityEngine;

public class Observer : MonoBehaviour
{
    public Transform player;
    public EnemyMovement enemyMovement;

    public float loseSightDelay = 3f;

    private bool m_IsPlayerInRange;
    private float loseSightTimer;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        if (enemyMovement == null)
        {
            enemyMovement = GetComponentInParent<EnemyMovement>();
        }

        loseSightTimer = loseSightDelay;
    }

    void OnTriggerEnter(Collider other)
    {
        if (player != null && (other.transform == player || other.transform.IsChildOf(player)))
        {
            m_IsPlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (player != null && (other.transform == player || other.transform.IsChildOf(player)))
        {
            m_IsPlayerInRange = false;
        }
    }

    void Update()
    {
        if (player == null || enemyMovement == null)
            return;

        bool canSeePlayer = false;

        if (m_IsPlayerInRange)
        {
            Vector3 direction = player.position - transform.position + Vector3.up;

            Debug.DrawRay(transform.position, direction, Color.green);

            Ray ray = new Ray(transform.position, direction);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit))
            {
                Debug.DrawLine(transform.position, raycastHit.point, Color.red);

                if (raycastHit.collider.transform == player || raycastHit.collider.transform.IsChildOf(player))
                {
                    canSeePlayer = true;
                }
            }
        }

        if (canSeePlayer)
        {
            loseSightTimer = loseSightDelay;
            enemyMovement.EnableChase();
        }
        else
        {
            loseSightTimer -= Time.deltaTime;

            if (loseSightTimer <= 0f)
            {
                enemyMovement.DisableChase();
            }
        }
    }
}