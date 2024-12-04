using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public static bool isSubMenuOpen = false;
    public GameObject gameMenu;
    public GameObject settingMenu;
    public static bool playerFrozen;

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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (!ComponentManager.Instance.winScreenIsDisplayed)
        {
            gameMenu.SetActive(true);
        }
    }

    public void ResumeGameWithGUI()
    {
        Time.timeScale = 1;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gameMenu.SetActive(false);
    }

    public static void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
}