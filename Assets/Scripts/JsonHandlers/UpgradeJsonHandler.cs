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

    public class Stats
    {

    }
    [System.Serializable]
    public class MaxHealth : Stats
    {
        public int level;
        public int maxHealth;
        public int cost;
    }
    [System.Serializable]
    public class Attack : Stats
    {
        public int level;
        public int attack;
        public float attackSpeed;
        public int cost;
    }
    [System.Serializable]
    public class EmergencyAllySpawn : Stats
    {
        public bool state;
        public int cost;
    }
    [System.Serializable]
    public class BuildingName : Stats
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
        public string upgradeName;
        public string upgradeIDName;
    }
    [System.Serializable]
    public class Root
    {
        public List<BuildingName> building;
    }

    public static Root ReadFile()
    {
        string rawData = File.ReadAllText(Application.streamingAssetsPath + "/upgrades.txt");
        Root data = JsonUtility.FromJson<Root>(rawData);
        return data;
    }
}