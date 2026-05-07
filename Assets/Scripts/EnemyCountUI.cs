using UnityEngine;
using TMPro;

public class EnemyCountUI : MonoBehaviour
{
    public TextMeshProUGUI enemyText;

    public void UpdateEnemyCount(int count)
    {
        enemyText.text = "Enemies Left: " + count;
    }
}