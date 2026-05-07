using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float distance = 8f;
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 offset = new Vector3(0f, distance, -distance);
        Vector3 desired = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(30f, 0f, 0f);
    }
}