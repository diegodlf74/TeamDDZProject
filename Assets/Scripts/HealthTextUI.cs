using UnityEngine;
using TMPro;

public class HealthTextUI : MonoBehaviour
{
    public TextMeshProUGUI healthText;

    public void UpdateHealth(int current, int max)
    {
        healthText.text = "HP: " + current + " / " + max;
    }
}