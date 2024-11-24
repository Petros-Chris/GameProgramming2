using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class JsonHandler : MonoBehaviour
{
    public class DataToSave
    {
        public int frameRate;
        public int vsyncOption;
    }

    public class WaveData
    {
        public String difficulty;
        public int wave;
        public int basicEnemy;
        public int count;
        public int advancedEnemy;
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string type;
        public int count;
    }
    [System.Serializable]
    public class DifficultyLevel
    {
        public string difficulty;
        public List<Wave> waves;
    }
    [System.Serializable]
    public class Wave
    {
        public int wave;
        public int totalEnemies;
        public List<EnemyGroup> enemies;
    }

    [System.Serializable]
    public class Root
    {
        public List<DifficultyLevel> difficulty;
    }

    [System.Serializable]
    public class Test
    {
        public int round;
        public List<EnemyGroup> enemy;
    }

    public static void Save(int frameRate = 0, int vsyncOption = 1)
    {
        Debug.Log("Saving to" + Application.dataPath);

        DataToSave dataToSave = new DataToSave
        {
            frameRate = frameRate,
            vsyncOption = vsyncOption,
        };

        string json = JsonUtility.ToJson(dataToSave);
        File.WriteAllText(Application.dataPath + "/settings.txt", json);
    }

    public static DataToSave ReadFile(String fileNameToRead)
    {
        String rawData = File.ReadAllText(Application.dataPath + "/" + fileNameToRead + ".txt");
        DataToSave data = JsonUtility.FromJson<DataToSave>(rawData);
        return data;
    }
    public static Root ReadFileForWave(String fileNameToRead = "wavesaaaa")
    {
        String rawData = Resources.Load<TextAsset>(fileNameToRead).text;

        Root data = JsonUtility.FromJson<Root>(rawData);
        return data;
    }
}