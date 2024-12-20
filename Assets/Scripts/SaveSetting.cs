using UnityEngine;
using System.IO;
public class SaveSetting : MonoBehaviour
{
    [System.Serializable]
    public class DataToSave
    {
        public bool displayFps;
        public bool fullScreen;
        public bool eggsMode;
        public int frameRate;
        public int vSyncLevel;
    }

    public static void Save(bool toggleFps, bool toggleFullScreen, bool toggleEggsMode, int changeFrameRate, int changeVSync)
    {
        Debug.Log("Saving User Settings to" + Application.streamingAssetsPath);

        DataToSave dataToSave = new DataToSave
        {
            displayFps = toggleFps,
            fullScreen = toggleFullScreen,
            eggsMode = toggleEggsMode,
            frameRate = changeFrameRate,
            vSyncLevel = changeVSync
        };

        string json = JsonUtility.ToJson(dataToSave);
        File.WriteAllText(Application.streamingAssetsPath + "/settings.txt", json);
    }

    public static DataToSave LoadUserSettings()
    {
        if (!File.Exists(Application.streamingAssetsPath + "/settings.txt"))
        {
            Save(false, false, false, 0, 0);
        }
        string rawData = File.ReadAllText(Application.streamingAssetsPath + "/settings.txt");
        DataToSave data = JsonUtility.FromJson<DataToSave>(rawData);
        return data;
    }
}
