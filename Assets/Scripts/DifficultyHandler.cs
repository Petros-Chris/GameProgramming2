using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    public void SelectDifficulty()
    {
        Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        switch (button.name)
        {
            case "EasyMode":
                Instance.difficultyLevel = "EasyMode";
                break;
            case "MediumMode":
                Instance.difficultyLevel = "MediumMode";
                break;
            case "HardMode":
                Instance.difficultyLevel = "HardMode";
                break;
            case "DeathMode":
                Instance.difficultyLevel = "DeathMode";
                break;
        }
        // Goes Next Level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
