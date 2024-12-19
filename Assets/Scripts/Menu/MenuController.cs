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
    public void ContinueWhereYouLeftOff()
    {
        SaveScene.DataToSave playerData = SaveScene.ReadSaveFile();

        // Im converting the string I made into a int back into a string here ;(
        switch (playerData.difficultyLevel)
        {
            case 0:
                DifficultyHandler.Instance.difficultyLevel = "EasyMode";
                break;
            case 1:
                DifficultyHandler.Instance.difficultyLevel = "MediumMode";
                break;
            case 2:
                DifficultyHandler.Instance.difficultyLevel = "HardMode";
                break;
            case 3:
                DifficultyHandler.Instance.difficultyLevel = "DeathMode";
                break;
        }
        // Exclude the last scene, which is currently set to be the lose screen
        int levelAmount = SceneManager.sceneCountInBuildSettings - 1;
        Debug.Log(levelAmount);
        Debug.Log(" HI HI player" + playerData.worldLevel);
        if (playerData.worldLevel >= levelAmount || playerData.worldLevel <= 0)
        {
            // Restart as the game is done, not started, or the file was tampered with
            StartGame();
            return;
        }
        // Loads the last level the player was in
        SceneManager.LoadScene(playerData.worldLevel);
    }
}

