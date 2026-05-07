using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    public GameObject levelExit;
    public EnemyCountUI enemyUI;

    private int enemiesAlive = 0;

    private void Start()
    {
        
        if (levelExit != null)
            levelExit.SetActive(false);

        enemiesAlive = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None).Length;

        Debug.Log("Enemies alive: " + enemiesAlive);

        enemyUI.UpdateEnemyCount(enemiesAlive);

        if (enemiesAlive <= 0)
            ShowExit();
    }

    public void EnemyDied()
    {
        enemiesAlive--;
        enemyUI.UpdateEnemyCount(enemiesAlive);
        Debug.Log("Enemy died. Remaining: " + enemiesAlive);

        if (enemiesAlive <= 0)
            ShowExit();
    }

    private void ShowExit()
    {
        if (levelExit != null)
            levelExit.SetActive(true);

        Debug.Log("All enemies defeated. Exit appeared!");
    }
}