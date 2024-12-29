using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    private Canvas mainCanvas;
    private Canvas playerCanvas;
    private Canvas weaponCanvas;
    private bool isShopMenuOpen;
    private Canvas[] canvases;
    private ComponentManager cmInstance;
    private GameMenu gmInstance;
    public float interactionRange = 5;
    public float unFocusTransparency = 0.4f;
    public float FocusTransparency = 1f;

    private void Start()
    {
        canvases = GetAllCanvases();
        AssignCanvasVariables();
        cmInstance = ComponentManager.Instance;
        gmInstance = GameMenu.Instance;
    }

    private void Update()
    {
        WasShopHit();
    }

    private void AssignCanvasVariables()
    {
        foreach (Canvas canvas in canvases)
        {
            switch (canvas.name)
            {
                case "MainShopMenu":
                    mainCanvas = canvas;
                    break;
                case "PlayerUpgradeMenu":
                    playerCanvas = canvas;
                    break;
                case "WeaponUpgradeMenu":
                    weaponCanvas = canvas;
                    break;
            }
        }
    }

    private void WasShopHit()
    {
        // If interact was pressed
        if (cmInstance.interactKey.triggered)
        {
            // If player is in player mode
            if (cmInstance.playerCam == null)
                return;

            // If player already has a menu open
            if (gmInstance.isInGameMenuOpen)
                return;

            Ray ray = cmInstance.playerCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If it hit anything within range
            if (Physics.Raycast(ray, out hit, interactionRange))
            {
                // If it didn't hit the shop
                if (!hit.collider.CompareTag("Shop"))
                {
                    return;
                }
                // If player is currently in a round
                if (cmInstance.lockCamera)
                {
                    StartCoroutine(cmInstance.ShowMessage("You Can't Access The Shop During A Round!"));
                    return;
                }
                AccessShop(hit.point);
            }
        }
    }

    private IEnumerator IsObjectStillInRange(Vector3 raycastPos)
    {
        while (cmInstance.IsInRange(interactionRange, cmInstance.playerCam.transform.position, raycastPos))
        {
            if (cmInstance.lockCamera)
            {
                break;
            }

            if (Cursor.lockState == CursorLockMode.Locked)
                cmInstance.FocusCursor(false);
            yield return true;
        }
        CleanUp();
    }

    private void DisableAllCanvases()
    {
        foreach (Canvas canvas in canvases)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    private void DisableAllActiveCanvases()
    {
        Canvas[] activeCanvases = GetAllActiveCanvases();
        foreach (Canvas canvas in activeCanvases)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    public void CloseShop()
    {
        StopAllCoroutines();
        CleanUp();
    }

    public void SwitchShopMenu()
    {
        Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        string buttonName = button.name.Replace("Btn", "");
        Canvas selectedCanvas = MatchNameToCanvas(buttonName);

        // Disables all and enables selected one
        DisableAllActiveCanvases();
        selectedCanvas.gameObject.SetActive(true);
    }

    private Canvas MatchNameToCanvas(string nameToMatch)
    {
        switch (nameToMatch)
        {
            case "Player":
                return playerCanvas;
            case "General":
                return mainCanvas;
            case "Weapon":
                return weaponCanvas;
        }
        return mainCanvas;
    }

    private void CleanUp()
    {
        gmInstance.isInGameMenuOpen = false;
        cmInstance.FocusCursor();
        DisableAllCanvases();
    }

    private void AccessShop(Vector3 raycastPos)
    {
        mainCanvas.gameObject.SetActive(true);
        gmInstance.isInGameMenuOpen = true;
        cmInstance.FocusCursor(false);
        StartCoroutine(IsObjectStillInRange(raycastPos));
    }

    private Canvas[] GetAllActiveCanvases()
    {
        return GetComponentsInChildren<Canvas>(false);
    }

    private Canvas[] GetAllCanvases()
    {
        return GetComponentsInChildren<Canvas>(true);
    }
}
