using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public float money;
    public float attention;
    public int currentPity;
    public List<ItemSaveData> items = new List<ItemSaveData>();
}

[Serializable]
public struct ItemSaveData
{
    public string id;      // ID из UpgradeConfig
    public int levelIndex;
}
