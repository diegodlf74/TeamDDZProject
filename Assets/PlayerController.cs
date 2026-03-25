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

    private Animator anim;

    [Header("Attack")]
    public float attackRange = 1.2f;
    public float attackRadius = 0.6f;
    public int attackDamage = 1;
    public LayerMask enemyLayers;
    public Transform attackPoint;

    [Header("Usable Power-Up")]
    public UsablePowerUpType equippedPowerUp = UsablePowerUpType.None;
    public float invisibilityDuration = 5f;
    public float usableSpeedMultiplier = 2f;
    public float usableSpeedDuration = 5f;

    private bool isInvisible = false;

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

    public void OnUseItem(InputValue value)
    {
        if (!value.isPressed) return;
        if (equippedPowerUp == UsablePowerUpType.None) return;

        switch (equippedPowerUp)
        {
            case UsablePowerUpType.Invisibility:
                StartCoroutine(InvisibilityRoutine());
                break;

            case UsablePowerUpType.SpeedBoost:
                AddSpeedModifier(usableSpeedMultiplier, usableSpeedDuration);
                break;
        }

        equippedPowerUp = UsablePowerUpType.None;
    }

    public void GiveUsablePowerUp(UsablePowerUpType powerUpType)
    {
        equippedPowerUp = powerUpType;
    }

    public bool IsInvisible()
    {
        return isInvisible;
    }

    float GetFinalSpeed()
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

    IEnumerator RemoveModifier(SpeedModifier mod)
    {
        yield return new WaitForSeconds(mod.duration);
        speedModifiers.Remove(mod);
    }

    IEnumerator InvisibilityRoutine()
    {
        if (isInvisible) yield break;

        isInvisible = true;

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                mat.SetFloat("_Surface", 1f);
                mat.SetFloat("_Blend", 0f);
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.renderQueue = 3000;

                if (mat.HasProperty("_BaseColor"))
                {
                    Color c = mat.GetColor("_BaseColor");
                    c.a = 0.2f;
                    mat.SetColor("_BaseColor", c);
                }
            }
        }

        yield return new WaitForSeconds(invisibilityDuration);

        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                mat.SetFloat("_Surface", 0f);
                mat.SetFloat("_Blend", 0f);
                mat.SetOverrideTag("RenderType", "Opaque");
                mat.renderQueue = -1;

                if (mat.HasProperty("_BaseColor"))
                {
                    Color c = mat.GetColor("_BaseColor");
                    c.a = 1f;
                    mat.SetColor("_BaseColor", c);
                }
            }
        }

        isInvisible = false;
    }

    void FixedUpdate()
    {
        Vector3 move = new Vector3(movementX, 0f, movementY);
        bool moving = move.sqrMagnitude > 0.01f;

        rb.angularVelocity = Vector3.zero;

        float finalSpeed = GetFinalSpeed();

        if (moving)
        {
            move = move.normalized;

            rb.linearVelocity = new Vector3(move.x * finalSpeed, rb.linearVelocity.y, move.z * finalSpeed);

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

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
