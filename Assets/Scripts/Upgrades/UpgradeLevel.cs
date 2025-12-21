using UnityEngine;

[System.Serializable]
public class UpgradeLevel
{
    public string levelName;
    public Sprite visualState;
    public float cost;

    [Header("Bonuses")]
    public float clickPowerBonus;
    public float passiveAttentionBonus;
    public float eventChanceBonus;
}