using UnityEngine;
using System.IO;

public class SaveScene : MonoBehaviour
{
    [System.Serializable]
    public class DataToSave
    {
        public int worldLevel;
        public int difficultyLevel;
    }

    public static void Save(int currentScene, int currentDifficultyLevel)
    {
        Debug.Log("Saving Progress to" + Application.streamingAssetsPath);

        DataToSave dataToSave = new DataToSave
        {
            worldLevel = currentScene,
            difficultyLevel = currentDifficultyLevel
        };

        string json = JsonUtility.ToJson(dataToSave);
        File.WriteAllText(Application.streamingAssetsPath + "/user.txt", json);
    }

    public static DataToSave ReadSaveFile()
    {
        if (!File.Exists(Application.streamingAssetsPath + "/user.txt"))
        {
            Save(0, 0);
        }
        string rawData = File.ReadAllText(Application.streamingAssetsPath + "/user.txt");
        DataToSave data = JsonUtility.FromJson<DataToSave>(rawData);
        return data;
    }
}
