using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public static GameMenu Instance { get; private set; }
    public bool isPaused = false;
    public bool isSubMenuOpen = false;
    public KeyCode pauseGame = KeyCode.Escape;
    public GameObject gameMenu;
    public GameObject settingMenu;
    public bool isUpdateMenuOpen = false;
    public bool playerFrozen;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gameMenu = GameObject.Find("Menu");
        gameMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isSubMenuOpen)
            {
                CloseSetting();
            }

            if (isPaused)
            {
                ResumeGameWithGUI();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
        ComponentManager.Instance.FocusCursor(false);
        if (!ComponentManager.Instance.winScreenIsDisplayed)
        {
            gameMenu.SetActive(true);
        }
    }

    public void ResumeGameWithGUI()
    {
        Time.timeScale = 1;
        isPaused = false;
        ComponentManager.Instance.FocusCursor();
        gameMenu.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        ComponentManager.Instance.FocusCursor();
    }

    public void AbandonKingdom()
    {
        ResumeGame();
        ComponentManager.Instance.FocusCursor(false);
        SceneManager.LoadScene(0);
    }

    public void OpenSetting()
    {
        gameMenu.SetActive(false);
        settingMenu.SetActive(true);
        isSubMenuOpen = true;
    }

    public void CloseSetting()
    {
        settingMenu.SetActive(false);
        gameMenu.SetActive(true);
        isSubMenuOpen = false;
    }

    public void ResetPlayerPosition()
    {
        GameObject player = GameObject.Find("Player");
        PlayerMovementAdvanced movement = player.GetComponent<PlayerMovementAdvanced>();
        GameObject fishKingdom = GameObject.Find("FishKingdom");

        // Don't need to check for kingdom as the level ends on the frame kingdom becomes null
        if (player == null)
        {
            ComponentManager.Instance.CallCoroutine(ComponentManager.Instance.ShowMessage("Can't Move; Player Is Dead!"));
            return;
        }
        // Stops the player from moving while a round is in progress (prevents using it to quickly get back to the kingdom)
        if (ComponentManager.Instance.lockCamera)
        {
            ComponentManager.Instance.CallCoroutine(ComponentManager.Instance.ShowMessage("Can't Move; Round Is In Progress!"));
            return;
        }
        // Create the point to spawn in front of kingdom
        Vector3 fkPos = fishKingdom.transform.position;
        Vector3 spawnPoint = new Vector3(fkPos.x, fkPos.y, fkPos.z - 3);
        player.transform.position = spawnPoint;
        // Reset player speed
        movement.moveSpeed = 0;
    }
}