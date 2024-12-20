using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public class UpgradeController : MonoBehaviour
{
    public GameObject upgradeCanvas;
    // will likely be used for shop as well (if we interact with a person for shop)
    public KeyCode interact = KeyCode.E;

    public float RayCastRange = 5f;
    public float RangeInBuildMode = 30f;
    private float RayRange;
    private string audioPath = "GUI";

    public GameObject textPrefab;
    public GameObject button;
    bool shouldValuesBeUpdated;
    Vector3 menuCreation;
    Camera cameraToUse;
    private Dictionary<string, Action<int, Button>> methodMap;
    private Building building;
    UpgradeJsonHandler.Root root;
    List<UpgradeJsonHandler.Upgrade> upgrades;


    void Start()
    {
        methodMap = new Dictionary<string, Action<int, Button>>()
        {
            { "Maximum Health", UpgradeBuildingHealth },
            { "Building Damage", UpgradeBuildingAttack },
            { "Spawn Allies In Trouble", AddSpawnInTroubleUpgrade }
        };
        root = UpgradeJsonHandler.ReadFile();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GetObjectWithRayCast();
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
                ComponentManager.Instance.FocusCursor();
            }
        }
    }

    void GetObjectWithRayCast()
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
            // Checks if player is currently allowed (lockCamera is in regards to when a round is in progress)
            if (ComponentManager.Instance.lockCamera)
            {
                StartCoroutine(ComponentManager.Instance.ShowMessage("You Can't Upgrade A Building During A Round!"));
                return;
            }
            UpgradeJsonHandler.BuildingName dataFromBuilding = default;

            building = default;
            if (hit.collider.TryGetComponent(out Kingdom target))
            {
                building = target;
                dataFromBuilding = root.building.Find(b => b.building == "Kingdom");
                upgrades = root.building.Find(b => b.building == "FishKingdom").upgrades;
            }
            else if (hit.collider.TryGetComponent(out Tower tower))
            {
                building = tower;
                dataFromBuilding = root.building.Find(b => b.building == "Tower");
                upgrades = root.building.Find(b => b.building == "Tower").upgrades;
            }
            else if (hit.collider.TryGetComponent(out Wall wall))
            {
                building = wall;
                dataFromBuilding = root.building.Find(b => b.building == "Wall");
                upgrades = root.building.Find(b => b.building == "Wall").upgrades;
            }
            else if (hit.collider.transform.parent.parent.TryGetComponent(out Building building1))
            {
                building = building1;
                dataFromBuilding = root.building.Find(b => b.building == "Wall");
                upgrades = root.building.Find(b => b.building == "Wall").upgrades;
            }

            if (building == default)
            {
                return;
            }


            CleanUpPastPanel();
            upgradeCanvas.SetActive(true);
            GameMenu.Instance.isUpdateMenuOpen = true; // Freeze player
            ComponentManager.Instance.FocusCursor(false); // Show mouse

            menuCreation = cameraToUse.transform.position;

            // Gets all public and local fields
            FieldInfo[] fields = building.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            StopAllCoroutines(); // For now 
            StartCoroutine(UpdateFields(fields, building, dataFromBuilding));
        }

    }
    IEnumerator UpdateFields(FieldInfo[] fields, Building building, UpgradeJsonHandler.BuildingName dataFromBuilding)
    {
        shouldValuesBeUpdated = true;
        while (shouldValuesBeUpdated)
        {
            float yOffset = 0;

            // Stop if round has started
            if (ComponentManager.Instance.lockCamera)
            {
                CloseUpgradeMenu();
            }
            CleanUpPastPanel();

            // Readds all children in the canvas
            foreach (var upgrade in upgrades)
            {
                GameObject fieldTextObj = Instantiate(textPrefab, upgradeCanvas.transform);
                TextMeshProUGUI fieldText = fieldTextObj.GetComponent<TextMeshProUGUI>();
                RectTransform rectTransform = fieldText.GetComponent<RectTransform>();

                // Sets the text spot
                rectTransform.anchorMin = new Vector2(1, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                rectTransform.pivot = new Vector2(2.2f, 0.5f);

                UpdateFields(fieldText, upgrade, yOffset);

                fieldText.rectTransform.anchoredPosition = new Vector2(0, yOffset);
                // CreateButton(upgrade.upgradeName, upgrade, yOffset + 10);
                yOffset -= 50;
            }
            // Waits a second before re updating it all
            yield return new WaitForSeconds(1);
        }
    }

    void UpdateFields(TextMeshProUGUI fieldText, UpgradeJsonHandler.Upgrade upgrade, float yOffset)
    {
        // Checks if attack is empty
        if (upgrade.attack.Count != 0)
        {
            int index = building.GetAttackLevel() - 1;
            int nextIndex = building.GetAttackLevel();

            if (nextIndex == upgrade.attack[^1].level)
            {
                fieldText.text = $"{upgrade.upgradeName}: {upgrade.attack[index].attack} -> {upgrade.attack[index].attack}:";
            }
            else
            {
                fieldText.text = $"{upgrade.upgradeName}: {upgrade.attack[index].attack} -> {upgrade.attack[nextIndex].attack} Costs: {upgrade.attack[nextIndex].cost}:";
            }
            CreateButton(upgrade.upgradeName, upgrade.attack[index].level, upgrade.attack[^1].level, upgrade, yOffset + 10);
        }
        // Checks if maxHealth is empty
        else if (upgrade.maxHealth.Count != 0)
        {
            int index = building.GetHealthLevel() - 1;
            int nextIndex = building.GetHealthLevel();

            // Checks if it's max level
            if (nextIndex == upgrade.maxHealth[^1].level)
            {
                fieldText.text = $"{upgrade.upgradeName}: {upgrade.maxHealth[index].maxHealth} -> {upgrade.maxHealth[index].maxHealth}:";
            }
            else
            {
                fieldText.text = $"{upgrade.upgradeName}: {upgrade.maxHealth[index].maxHealth} -> {upgrade.maxHealth[nextIndex].maxHealth} Costs: {upgrade.maxHealth[nextIndex].cost}:";
            }
            // Creates a button with upgrade data
            CreateButton(upgrade.upgradeName, upgrade.maxHealth[index].level, upgrade.maxHealth[^1].level, upgrade, yOffset + 10);
        }
        // Checks if emergencyAllySpawn is empty
        else if (upgrade.emergencyAllySpawn.Count != 0)
        {
            int index = building.GetEmergencyAllySpawn() - 1;
            int nextIndex = building.GetEmergencyAllySpawn();

            // Checks if it's max level
            if (nextIndex == 2) // Its boolean so anything higher than 1 is impossible
            {
                fieldText.text = $"{upgrade.upgradeName}: {upgrade.emergencyAllySpawn[index].state} -> {upgrade.emergencyAllySpawn[index].state}:";
            }
            else
            {
                fieldText.text = $"{upgrade.upgradeName}: {upgrade.emergencyAllySpawn[index].state} -> {upgrade.emergencyAllySpawn[nextIndex].state} Costs: {upgrade.emergencyAllySpawn[nextIndex].cost}:";
            }
            CreateButton(upgrade.upgradeName, upgrade.emergencyAllySpawn[index].state ? 1 : 0, 1, upgrade, yOffset + 10);
        }
    }

    void CreateButton(string nameOfUpgrade, int currentLevel, int maxLevel, UpgradeJsonHandler.Upgrade upgrade, float yOffset)
    {
        // if (nameOfUpgrade == "health")
        // {
        //     return;
        // }
        int xOffset = -200;
        // int buttonLvl = 0;

        GameObject upgradeBtnPreFab = Instantiate(button, upgradeCanvas.transform);
        TextMeshProUGUI btnText = upgradeBtnPreFab.GetComponentInChildren<TextMeshProUGUI>();
        RectTransform rectTransform = upgradeBtnPreFab.GetComponent<RectTransform>();
        Button upgradeBtn = upgradeBtnPreFab.GetComponent<Button>();

        // Sets the button spot
        rectTransform.anchorMin = new Vector2(1, 0.5f);
        rectTransform.anchorMax = new Vector2(1, 0.5f);
        rectTransform.pivot = new Vector2(0, 0.5f);
        btnText.text = $"Upgrade \n {currentLevel}/{maxLevel}";

        rectTransform.anchoredPosition = new Vector2(xOffset, yOffset);
        xOffset += 50;

        if (methodMap.ContainsKey(nameOfUpgrade))
        {
            upgradeBtn.onClick.AddListener(() => methodMap[nameOfUpgrade](0, upgradeBtn));
            // buttonLvl++;
        }
        //upgradeBtn.OnPointerEnter
        else
        {
            Debug.Log("No method found for upgrade: " + nameOfUpgrade);
        }
    }

    void UpgradeBuildingHealth(int ch, Button button)
    {
        SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 1f);
        if (building.SetMaxHealth(upgrades[0].maxHealth))
        {
            //TODO: Somehow make the button remmeber last state
            // button.interactable = false;
            // ColorBlock colors = button.colors;
            // colors.normalColor = Color.green;
            // button.colors = colors;
        }
    }
    void UpgradeBuildingAttack(int ch, Button button)
    {
        SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 1f);
        // This 1 took me 40 mins to debug :"(
        if (building.SetAttack(upgrades[1].attack))
        {
            //TODO: Somehow make the button remmeber last state
        }
    }
    void AddSpawnInTroubleUpgrade(int ch, Button button)
    {
        SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 1f);
        if (building is Kingdom kingdom)
        {
            kingdom.SetEmergencyAllySpawn(upgrades[2].emergencyAllySpawn);
        }
    }
    Camera WhichCameraInUse()
    {
        // Doesn't check if death camera is active

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
        SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 1f);
        upgradeCanvas.SetActive(false);
        shouldValuesBeUpdated = false;
        GameMenu.Instance.isUpdateMenuOpen = false;
        ComponentManager.Instance.FocusCursor();
    }
    public bool IsPlayerInMenuCreationRange()
    {
        float distanceToPlayer = Vector3.Distance(cameraToUse.transform.position, menuCreation);
        return distanceToPlayer <= RayRange;
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
}
