using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using Unity.AI.Navigation;
using TMPro;

public class ComponentManager : MonoBehaviour
{
    public static ComponentManager Instance { get; private set; }
    public Camera buildCam;
    public Camera playerCam;
    public Camera deathCam;
    public KeyCode switchModes = KeyCode.M;
    public GameObject defaultEnemy;
    public GameObject fastEnemy;
    public GameObject tankEnemy;

    // I feel like all this should be in gameMenu 
    public bool lockCamera;
    public GameObject message;
    public TextMeshProUGUI messageText;
    public GameObject panel;
    public GameObject winCanvas;
    public bool winScreenIsDisplayed;

    void Start()
    {
        messageText = message.GetComponent<TextMeshProUGUI>();
        fastEnemy = Resources.Load<GameObject>("PreFabs/Characters/Enemies/FastEnemy");
        defaultEnemy = Resources.Load<GameObject>("PreFabs/Characters/Enemies/RangedEnemy");
        tankEnemy = Resources.Load<GameObject>("PreFabs/Characters/Enemies/TankEnemy");

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
                GameMenu.playerFrozen = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                buildCam.gameObject.SetActive(false);
                playerCam.gameObject.SetActive(true);
            }
            else
            {
                GameMenu.playerFrozen = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                buildCam.gameObject.SetActive(true);
                playerCam.gameObject.SetActive(false);
            }
        }
    }
    public void SwitchToPlayerAndLockCamera()
    {
        GameMenu.playerFrozen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        lockCamera = true;
        buildCam.gameObject.SetActive(false);
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

    public void CallCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public void DisplayWinScreen()
    {
        winCanvas.SetActive(true);
        winScreenIsDisplayed = true;
    }
}