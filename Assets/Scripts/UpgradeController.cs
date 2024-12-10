using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using TMPro;
using UnityEngine.UI;
using System;

public class UpgradeController : MonoBehaviour
{
    public GameObject upgradeCanvas;
    // will likely be used for shop as well (if we interact with a person for shop)
    public KeyCode interact = KeyCode.E;

    public float RayCastRange = 5f;
    public float RangeInBuildMode = 30f;
    private float RayRange;

    public GameObject textPrefab;
    public GameObject button;
    bool shouldValuesBeUpdated;
    Vector3 menuCreation;
    Camera cameraToUse;
    private Dictionary<string, UnityEngine.Events.UnityAction> methodMap;
    private Building building;


    void Start()
    {
        methodMap = new Dictionary<string, UnityEngine.Events.UnityAction>
        {
            { "maxHealth", UpgradeBuildingHealth }
        };
        // upgradeCanvas = GameObject.Find("UpgradeCanvas");
    }

    void Update()
    {
        //TODO: should freeze player, or time, or neither

        // Stop from checking upgrades in build cam Unlesss !
        // if (ComponentManager.Instance.)
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (ComponentManager.Instance.lockCamera)
            {
                StartCoroutine(ComponentManager.Instance.ShowMessage("You Can't Upgrade A Building During A Round!"));
                return;
            }
            GetObject();
        }

        if (menuCreation != default)
        {
            if (cameraToUse == default)
            {
                cameraToUse = WhichCameraInUse();
                return;
            }

            if (!IsPlayerInMenuCreationRange())
            {
                // So it stops checking the distance
                menuCreation = default;
                CloseUpgradeMenu();
                FocusCursor();
            }
        }
    }

    public bool IsPlayerInMenuCreationRange()
    {
        float distanceToPlayer = Vector3.Distance(cameraToUse.transform.position, menuCreation);
        return distanceToPlayer <= RayRange;
    }

    void GetObject()
    {
        cameraToUse = WhichCameraInUse();

        // Neither of those cameras are active
        if (cameraToUse == default)
        {
            return;
        }

        Ray ray = cameraToUse.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, RayRange, LayerMask.GetMask("whatIsBuilding")))
        {
            building = default;
            if (hit.collider.TryGetComponent(out Kingdom target))
            {
                building = target;
            }
            else if (hit.collider.TryGetComponent(out Tower tower))
            {
                building = tower;
            }

            if (building == default)
            {
                return;
            }
            CleanUpPastPanel();
            upgradeCanvas.SetActive(true);
            GameMenu.isUpdateMenuOpen = true;
            FocusCursor(false);

            menuCreation = cameraToUse.transform.position;

            // Gets all public and local fields
            FieldInfo[] fields = building.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            StopAllCoroutines(); // For now 
            StartCoroutine(UpdateFields(fields, building));
        }

    }
    void CleanUpPastPanel()
    {
        // Destroys all children in the canvas except Panel
        foreach (Transform child in upgradeCanvas.transform)
        {
            if (child.gameObject.name != "Panel" && child.gameObject.name != "CloseBtn")
            {
                Destroy(child.gameObject);
            }
        }
    }

    // Catch position of when camera pressed, so when camera moves a certain distance away from pos, it closes the upgrade menu
    IEnumerator UpdateFields(FieldInfo[] fields, Building building)
    {
        shouldValuesBeUpdated = true;
        while (shouldValuesBeUpdated)
        {
            float yOffset = 0;

            if (ComponentManager.Instance.lockCamera)
            {
                CloseUpgradeMenu();
            }

            CleanUpPastPanel();
            // Readds all children in the canvas
            foreach (FieldInfo field in fields)
            {
                GameObject fieldTextObj = Instantiate(textPrefab, upgradeCanvas.transform);
                TextMeshProUGUI fieldText = fieldTextObj.GetComponent<TextMeshProUGUI>();
                RectTransform rectTransform = fieldText.GetComponent<RectTransform>();

                // Sets the text spot
                rectTransform.anchorMin = new Vector2(1, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                rectTransform.pivot = new Vector2(2.2f, 0.5f);

                fieldText.text = $"{field.Name}: {field.GetValue(building)}";

                fieldText.rectTransform.anchoredPosition = new Vector2(0, yOffset);
                CreateButton(field.Name, yOffset + 10);
                yOffset -= 50;
            }
            // Waits a second before re updating it all
            yield return new WaitForSeconds(1);
        }
    }

    void CreateButton(string nameOfUpgrade, float yOffset)
    {
        if (nameOfUpgrade == "health")
        {
            return;
        }

        GameObject upgradeBtnPreFab = Instantiate(button, upgradeCanvas.transform);
        TextMeshProUGUI btnText = upgradeBtnPreFab.GetComponentInChildren<TextMeshProUGUI>();
        RectTransform rectTransform = upgradeBtnPreFab.GetComponent<RectTransform>();
        Button upgradeBtn = upgradeBtnPreFab.GetComponent<Button>();

        // Sets the button spot
        rectTransform.anchorMin = new Vector2(1, 0.5f);
        rectTransform.anchorMax = new Vector2(1, 0.5f);
        rectTransform.pivot = new Vector2(1.5f, 0.5f);
        btnText.text = nameOfUpgrade;

        rectTransform.anchoredPosition = new Vector2(0, yOffset);

        if (methodMap.ContainsKey(nameOfUpgrade))
        {
            upgradeBtn.onClick.AddListener(methodMap[nameOfUpgrade]);
        }
        else
        {
            Debug.Log("No method found for upgrade: " + nameOfUpgrade);
        }
    }

    void UpgradeBuildingHealth()
    {
        switch (building)
        {
            case Kingdom kingdom:
                kingdom.UpgradeMaxHealth();
                break;
            case Tower tower:
                tower.UpgradeMaxHealth();
                break;
        }
    }

    // Doesn't check if death camera is active
    Camera WhichCameraInUse()
    {
        // Checks if build camera is currently in use
        if (ComponentManager.Instance.buildCam.gameObject.activeSelf)
        {
            RayRange = RangeInBuildMode;
            return ComponentManager.Instance.buildCam;
        }
        // Or else check if player camera is currently in use
        else if (ComponentManager.Instance.playerCam.gameObject.activeSelf)
        {
            RayRange = RayCastRange;
            return ComponentManager.Instance.playerCam;
        }
        return default;
    }

    public void CloseUpgradeMenu()
    {
        upgradeCanvas.SetActive(false);
        shouldValuesBeUpdated = false;
        GameMenu.isUpdateMenuOpen = false;
        FocusCursor();
    }

    void FocusCursor(bool shouldI = true)
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
}
