using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public Transform[] waypoints;

    private Rigidbody m_RigidBody;
    int m_CurrentWaypointIndex;

    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Transform currentWaypoint = waypoints[m_CurrentWaypointIndex];
        Vector3 currentToTarget = currentWaypoint.position - m_RigidBody.position;

        if (currentToTarget.magnitude < 0.1f)
        {
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        }
        Quaternion forwardRotation = Quaternion.LookRotation(currentToTarget);
        m_RigidBody.MoveRotation(forwardRotation);
        m_RigidBody.MovePosition(m_RigidBody.position + currentToTarget.normalized * moveSpeed * Time.deltaTime);
    }
}