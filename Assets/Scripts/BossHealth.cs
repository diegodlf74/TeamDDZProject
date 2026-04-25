using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int objectsRequired = 3;
    private int objectsDestroyed = 0;

    public GameObject shieldVisual;

    public void ObjectDestroyed()
    {
        objectsDestroyed++;

        Debug.Log("Boss object destroyed: " + objectsDestroyed + "/" + objectsRequired);

        if (objectsDestroyed >= objectsRequired)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Boss defeated!");
        Destroy(gameObject);
    }
}