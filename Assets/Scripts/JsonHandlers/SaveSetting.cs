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
        public float masterVolume;
        public float masterFxVolume;
        public float musicVolume;
    }

    public static void Save(bool toggleFps, bool toggleFullScreen, bool toggleEggsMode, int changeFrameRate, int changeVSync, float changeMasterVolume, float changeMasterFxVolume, float changeMusicVolume)
    {
        Debug.Log("Saving User Settings to" + Application.streamingAssetsPath);

        DataToSave dataToSave = new DataToSave
        {
            displayFps = toggleFps,
            fullScreen = toggleFullScreen,
            eggsMode = toggleEggsMode,
            frameRate = changeFrameRate,
            vSyncLevel = changeVSync,
            masterVolume = changeMasterVolume,
            masterFxVolume = changeMasterFxVolume,
            musicVolume = changeMusicVolume
        };

        string json = JsonUtility.ToJson(dataToSave);
        File.WriteAllText(Application.streamingAssetsPath + "/settings.txt", json);
    }

    public static DataToSave LoadUserSettings()
    {
        if (!File.Exists(Application.streamingAssetsPath + "/settings.txt"))
        {
            Save(false, false, false, 0, 0, 1, 1, 1);
        }
        string rawData = File.ReadAllText(Application.streamingAssetsPath + "/settings.txt");
        DataToSave data = JsonUtility.FromJson<DataToSave>(rawData);
        return data;
    }
}
