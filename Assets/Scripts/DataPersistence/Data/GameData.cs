using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int deathCount;

    public Vector3 lastCheckpointPosition;

    public SerializableDictionary<string, bool> gemsCollected;

    public int lastPlayedLevel;

    // Gia tri duoc dinh nghia trong ham nay se la gia tri mac dinh
    // Game bat dau khi khong co data de load
    public GameData()
    {
        this.deathCount = 0;
        
        lastCheckpointPosition = Vector3.zero;
        gemsCollected = new SerializableDictionary<string, bool>();

        this.lastPlayedLevel = 1; // Mặc định là màn chơi đầu tiên
    }
}
