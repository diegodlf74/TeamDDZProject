using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    public Transform player;
    bool m_IsPlayerInRange;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            m_IsPlayerInRange = false;
        }
    }

    void Update()
    {
        if (m_IsPlayerInRange)
        {
            Vector3 direction = player.position - transform.position + Vector3.up;

            // Draw the ray (green line)
            Debug.DrawRay(transform.position, direction, Color.green);

            Ray ray = new Ray(transform.position, direction);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit))
            {
                // Draw hit ray in red up to the hit point
                Debug.DrawLine(transform.position, raycastHit.point, Color.red);

                if (raycastHit.collider.transform == player)
                {
                    Debug.Log("Player was caught!");
                }
            }
        }
    }
}