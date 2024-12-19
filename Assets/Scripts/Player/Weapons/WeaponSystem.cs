using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class WeaponSystem : MonoBehaviour
{
    public Button button;
    public Canvas inventory;
    ColorBlock defaultColorBlock;
    ColorBlock equippedColorBlock;

    void Start()
    {
        DefaultEquip();
    }

    public void Update()
    {
        OpenInventory();
    }

    public void SwitchItem()
    {
        // Makes old button regular color again
        if (button != default)
        {
            button.colors = defaultColorBlock;
        }


        button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        // Gets regular color
        defaultColorBlock = button.colors;
        // Changes color to the equipped color
        equippedColorBlock = button.colors;
        equippedColorBlock.normalColor = Color.green;
        equippedColorBlock.highlightedColor = Color.green;
        equippedColorBlock.pressedColor = Color.green;
        equippedColorBlock.selectedColor = Color.green;
        button.colors = equippedColorBlock;

        string gunName = button.name.Replace("Btn", "");

        GameObject cameraPos = GameObject.Find("CameraPos");
        Transform[] inActiveChildren = cameraPos.GetComponentsInChildren<Transform>(true);
        Transform[] activeChildren = cameraPos.GetComponentsInChildren<Transform>(false);

        foreach (Transform inActiveChild in inActiveChildren)
        {
            if (inActiveChild.name == gunName)
            {
                foreach (Transform activeChild in activeChildren)
                {
                    if (activeChild == cameraPos.transform)
                    {
                        continue;
                    }
                    activeChild.gameObject.SetActive(false);
                    // Only one weapon should be active so escape after disabling one
                    break;
                }
                inActiveChild.gameObject.SetActive(true);
            }
        }
    }

    public void DefaultEquip()
    {
        defaultColorBlock = button.colors;
        equippedColorBlock = button.colors;
        equippedColorBlock.normalColor = Color.green;
        equippedColorBlock.highlightedColor = Color.green;
        equippedColorBlock.pressedColor = Color.green;
        equippedColorBlock.selectedColor = Color.green;
        button.colors = equippedColorBlock;

        string gunName = button.name.Replace("Btn", "");

        GameObject cameraPos = GameObject.Find("CameraPos");
        Transform[] inActiveChildren = cameraPos.GetComponentsInChildren<Transform>(true);

        foreach (Transform inActiveChild in inActiveChildren)
        {
            if (inActiveChild.name == gunName)
            {
                inActiveChild.gameObject.SetActive(true);
            }
        }
    }

    public void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventory.gameObject.SetActive(true);
            ComponentManager.Instance.FocusCursor(false);
            GameMenu.Instance.isUpdateMenuOpen = true;
            StartCoroutine(WhileInventoryOpen());
        }
    }

    public IEnumerator WhileInventoryOpen()
    {
        while (Input.GetKey(KeyCode.Tab))
        {
            yield return null;
        }
        inventory.gameObject.SetActive(false);
        ComponentManager.Instance.FocusCursor();
        GameMenu.Instance.isUpdateMenuOpen = false;
    }
}