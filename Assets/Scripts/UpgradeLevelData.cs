using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeLevel", menuName = "Clicker/UpgradeLevel")]
public class UpgradeLevelData : ScriptableObject
{
    public Sprite visualState; // Как выглядит предмет на этом уровне
    public float cost;

    [Header("Bonuses")]
    public float clickPowerBonus;
    public float passiveAttentionBonus; // Может быть отрицательным или положительным
    public float donationChanceMultiplier = 1.0f;
}