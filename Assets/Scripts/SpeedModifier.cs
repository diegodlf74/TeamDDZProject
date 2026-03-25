using UnityEngine;

[System.Serializable]
public class SpeedModifier
{
    public float multiplier;
    public float duration;

    public SpeedModifier(float multiplier, float duration)
    {
        this.multiplier = multiplier;
        this.duration = duration;
    }
}
