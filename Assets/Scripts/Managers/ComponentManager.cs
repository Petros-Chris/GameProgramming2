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
    public static GameObject defaultEnemy;
    public static GameObject fastEnemy;
    public static GameObject tankEnemy;

    void Start()
    {
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