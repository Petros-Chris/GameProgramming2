using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class ComponentManager : MonoBehaviour
{
    public static Camera buildCam;
    public static Camera playerCam;
    public static Camera deathCam;
    public static KeyCode switchModes = KeyCode.M;

    void Start()
    {
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

    public static void ToggleBuildPlayerMode()
    {
        if (Input.GetKeyDown(switchModes))
        {
            GameMenu.playerFrozen = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ComponentManager.buildCam.gameObject.SetActive(false);
            ComponentManager.playerCam.gameObject.SetActive(true);
        }
    }
}