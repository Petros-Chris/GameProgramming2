using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeJsonHandler : MonoBehaviour
{
    [System.Serializable]
    public class MaxHealth
    {
        public int level;
        public int maxHealth;
        public int cost;
    }
    [System.Serializable]
    public class Attack
    {
        public int level;
        public int attack;
        public int attackSpeed;
        public int cost;
    }
    [System.Serializable]
    public class EmergencyAllySpawn
    {
        public bool state;
        public int cost;
    }
    [System.Serializable]
    public class BuildingName
    {
        public string building;
        public List<Upgrade> upgrades;
    }
    [System.Serializable]
    public class Upgrade
    {
        public List<MaxHealth> maxHealth;
        public List<Attack> attack;
        public List<EmergencyAllySpawn> emergencyAllySpawn;
    }
    [System.Serializable]
    public class Root
    {
        public List<BuildingName> building;
    }

    // public static DataToSave ReadFile(String fileNameToRead)
    // {
    //     String rawData = File.ReadAllText(Application.dataPath + "/" + fileNameToRead + ".txt");
    //     DataToSave data = JsonUtility.FromJson<DataToSave>(rawData);
    //     return data;
    // }
    public static Root ReadFile()
    {
        string rawData = Resources.Load<TextAsset>("upgrades").text;

        Root data = JsonUtility.FromJson<Root>(rawData);
        return data;
    }
}