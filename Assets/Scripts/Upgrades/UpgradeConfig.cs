using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Clicker/UpgradeItem")]
public class UpgradeConfig : ScriptableObject
{
    public string id;
    public UpgradeLevel[] levels;
}