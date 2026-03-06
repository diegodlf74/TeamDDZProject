using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;



public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    private float movementX;
    private float movementY;

    public float speed = 0;

    public float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;

    private Animator anim;

    [Header("Attack")]
    public float attackRange = 1.2f;
    public float attackRadius = 0.6f;
    public int attackDamage = 1;
    public LayerMask enemyLayers;
    public Transform attackPoint; // empty object in front of player

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void OnMove(InputValue movementValue)
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
        anim.SetTrigger("Attack");

        Invoke(nameof(DoAttackHit), 0.1f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 move = new Vector3(movementX, 0f, movementY);
        bool moving = move.sqrMagnitude > 0.01f;

        // Stop any physics spinning
        rb.angularVelocity = Vector3.zero;

        if (moving)
        {
            rb.linearVelocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);

            Quaternion targetRot = Quaternion.LookRotation(move);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, 10f * Time.fixedDeltaTime));
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

        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius, enemyLayers);

        foreach (Collider hit in hits)
        {
            EnemyHealth health = hit.GetComponentInParent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(attackDamage);
            }
        }
    }
}
