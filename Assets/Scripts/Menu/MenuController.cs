using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static bool didKingdomDie;

    public void StartGame()
    {
        SaveScene.Save(0, 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GoNextLevel()
    {
        GameMenu.Instance.ResumeGame();
        ComponentManager.Instance.FocusCursor(false);
        // Go back to main menu as there is no game left (skips loseScreen)
        int levelAmount = SceneManager.sceneCountInBuildSettings - 2;
        Debug.Log(levelAmount);
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        if (SceneManager.GetActiveScene().buildIndex == levelAmount)
        {
            SceneManager.LoadScene(0);
            return;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        GameMenu.Instance.ResumeGame();
        didKingdomDie = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

