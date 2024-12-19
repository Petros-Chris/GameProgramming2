using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ComponentManager : MonoBehaviour
{
    public static ComponentManager Instance { get; private set; }
    public List<GameObject> buildingsDisabled;
    public bool hasPlayerDied;
    public Camera buildCam;
    public Camera playerCam;
    public Camera deathCam;
    public KeyCode switchModes = KeyCode.M;
    public GameObject defaultEnemy;
    public GameObject fastEnemy;
    public GameObject tankEnemy;
    public GameObject defaultAlly;

    // I feel like all this should be in gameMenu 
    public bool lockCamera;
    public GameObject message;
    public TextMeshProUGUI messageText;
    public GameObject panel;
    public GameObject winCanvas;
    public bool winScreenIsDisplayed;
    public string difficultyLevel;
    void Awake()
    {
        messageText = message.GetComponent<TextMeshProUGUI>();
        fastEnemy = Resources.Load<GameObject>("PreFabs/Characters/Enemies/FastEnemy");
        defaultEnemy = Resources.Load<GameObject>("PreFabs/Characters/Enemies/RangedEnemy");
        tankEnemy = Resources.Load<GameObject>("PreFabs/Characters/Enemies/TankEnemy");
        defaultAlly = Resources.Load<GameObject>("PreFabs/Characters/RangedAlly");

        Camera[] cameras = FindObjectsOfType<Camera>(true);
        foreach (var camera in cameras)
        {
            switch (camera.gameObject.name)
            {
                case "BuildCamera":
                    buildCam = camera;
                    break;
                case "Player Camera":
                    playerCam = camera;
                    break;
                case "Death Cam":
                    deathCam = camera;
                    break;
            }
        }
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReAssignCameras()
    {
        // Simply reassigns all cameras if player goes missing
        Camera[] cameras = FindObjectsOfType<Camera>(true);
        foreach (var camera in cameras)
        {
            switch (camera.gameObject.name)
            {
                case "BuildCamera":
                    buildCam = camera;
                    break;
                case "Player Camera":
                    playerCam = camera;
                    break;
                case "Death Cam":
                    deathCam = camera;
                    break;
            }
        }
    }

    public void ToggleBuildPlayerMode()
    {
        if (Input.GetKeyDown(switchModes))
        {
            if (lockCamera)
            {
                StartCoroutine(ShowMessage("You Can't Go Into Build Mode During A Round!"));
                return;
            }

            // Switch cam back to player if true
            if (buildCam.gameObject.activeSelf)
            {
                GameMenu.Instance.playerFrozen = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                buildCam.gameObject.SetActive(false);
                playerCam.gameObject.SetActive(true);
            }
            else
            {
                GameMenu.Instance.playerFrozen = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                buildCam.gameObject.SetActive(true);
                playerCam.gameObject.SetActive(false);
            }
        }
    }
    public void SwitchToPlayerAndLockCamera(bool localLockCamera = true)
    {
        GameMenu.Instance.playerFrozen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (localLockCamera)
        {
            lockCamera = true;
        }
        // Checks if death cam is active before disabling
        if (deathCam.gameObject.activeSelf)
        {
            deathCam.gameObject.SetActive(false);
        }
        // Checks if build cam is active before disabling
        if (buildCam.gameObject.activeSelf)
        {
            buildCam.gameObject.SetActive(false);
        }

        playerCam.gameObject.SetActive(true);
    }

    public IEnumerator ShowMessage(string errorMessage)
    {
        message.SetActive(true);
        panel.SetActive(true);
        messageText.text = errorMessage;
        yield return new WaitForSeconds(1);
        message.SetActive(false);
        panel.SetActive(false);
    }

    public IEnumerator Reload(Weapon weapon)
    {
        weapon.isReloading = true;
        yield return new WaitForSeconds(weapon.reloadTime);
        weapon.currentBullets = weapon.magazineSize;
        weapon.isReloading = false;
    }

    public void CallCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public void DisplayWinScreen()
    {
        winCanvas.SetActive(true);
        winScreenIsDisplayed = true;
    }
    public void FocusCursor(bool shouldI = true)
    {
        if (shouldI)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SelectDifficulty()
    {
        Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        switch (button.name)
        {
            case "EasyMode":
                difficultyLevel = "EasyMode";
                break;
            case "MediumMode":
                difficultyLevel = "MediumMode";
                break;
            case "HardMode":
                difficultyLevel = "HardMode";
                break;
            case "DeathMode":
                difficultyLevel = "DeathMode";
                break;
        }
        // Goes Next Level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public int ReturnDifficultyInInt()
    {
        switch (difficultyLevel)
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

    public void ContinueWhereYouLeftOff()
    {
        SaveScene.DataToSave playerData = SaveScene.ReadSaveFile();

        // Im converting the string I made into a int back into a string here ;(
        switch (playerData.difficultyLevel)
        {
            case 0:
                difficultyLevel = "EasyMode";
                break;
            case 1:
                difficultyLevel = "MediumMode";
                break;
            case 2:
                difficultyLevel = "HardMode";
                break;
            case 3:
                difficultyLevel = "DeathMode";
                break;
        }
        // Exclude the last scene, which is currently set to be the lose screen
        int levelAmount = SceneManager.sceneCountInBuildSettings - 1;
        if (playerData.worldLevel >= levelAmount || playerData.worldLevel < 0)
        {
            // Restart as the game is done or the file was tampered with
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            SaveScene.Save(0, 0);
        }
        // Loads the last level the player was in
        SceneManager.LoadScene(playerData.worldLevel);
    }
}