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

    void FixedUpdate()
    {
        Vector3 move = new Vector3(movementX, 0f, movementY);
        bool moving = move.sqrMagnitude > 0.01f;

        rb.angularVelocity = Vector3.zero;

        float finalSpeed = GetFinalSpeed();

        if (moving)
        {
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
}
