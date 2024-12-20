using UnityEngine;

public class DifficultyHandler : MonoBehaviour
{
    public static DifficultyHandler Instance { get; private set; }
    public string difficultyLevel = "MediumMode";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int ReturnDifficultyInInt()
    {
        switch (Instance.difficultyLevel)
        {
            case "EasyMode":
                return 0;
            case "MediumMode":
                return 1;
            case "HardMode":
                return 2;
            case "DeathMode":
                return 3;
        }
        // Defaults to normal
        return 1;
    }
}
