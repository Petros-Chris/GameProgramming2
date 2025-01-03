using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using Unity.VisualScripting;

public class BuildController : MonoBehaviour
{
    //? Tower Idea: Mini wall that is weaker but easier to place around map
    // if game lose while player in build mode, player frozen next game start ... Is this a issue? Player can't lose in build mode as can't build during round
    // Will not update current enemies on new towers, is that bad? 

    [Header("Controls")]
    public KeyCode spawnMultiple = KeyCode.RightShift;
    public KeyCode cancel = KeyCode.C;
    public KeyCode rotateBuildingLeft = KeyCode.Q;
    public KeyCode rotateBuildingRight = KeyCode.E;

    private GameObject tower;
    private GameObject outline;
    private bool shouldOutline;
    private bool shouldOutlineDelete;
    private Button button;
    private Renderer render;
    private int rotateAngle;
    private Quaternion angleToSpawnTower;
    private int priceOfObject;
    private int costRemaining;
    private Vector3 outOfBounds = new Vector3(0, -1000, 0);
    public Material materiasdal;
    private string audioPath = "GUI";
    private string audioPath2 = "BuildingPlaced";

    void Start()
    {
        outline = GameObject.Find("Outline");
        render = outline.GetComponent<Renderer>();
    }

    void OnDisable()
    {
        // Setting the outline out of bounds when not required anymore
        outline.transform.position = outOfBounds;

    }

    private IEnumerator SpawnTowerRoutine()
    {
        while (Input.GetKey(spawnMultiple) && !Input.GetKeyDown(GameMenu.Instance.pauseGame))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (CurrencyManager.Instance.Currency >= priceOfObject)
                {

                    SpawnTowerAtMouse();
                }
                else
                {
                    costRemaining = priceOfObject - CurrencyManager.Instance.Currency;
                    ComponentManager.Instance.CallCoroutine(ComponentManager.Instance.ShowMessage("You need " + costRemaining + "$ more"));
                    ComponentManager.Instance.CallCoroutine(FlashButton(Color.red));
                    break;
                }
            }
            // Changes color to show you are in multi place mode
            HighlightButton(true, button);
            yield return null;
        }
        shouldOutline = false;
        HighlightButton(false, button);
    }

    private IEnumerator TowerOutline()
    {
        while (shouldOutline && !Input.GetKeyDown(cancel))
        {
            // Deselects the button when user pauses the game
            if (Input.GetKeyDown(GameMenu.Instance.pauseGame))
            {
                EventSystem.current.SetSelectedGameObject(null);
                break;
            }
            Ray ray = ComponentManager.Instance.buildCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                outline.transform.position = hit.point;
            }

            RotateBuilding();

            if (IsCorrectArea())
            {
                render.material.SetColor("_EmissionColor", Color.green * 0.7f);
            }
            else
            {
                render.material.SetColor("_EmissionColor", Color.red * 0.7f);
            }
            yield return null;
        }
        // Setting the outline out of bounds
        outline.transform.position = outOfBounds;

        // If you want it to always go back to default angle
        rotateAngle = 0;
        outline.transform.rotation = Quaternion.Euler(0, 0, 0);
        angleToSpawnTower = Quaternion.Euler(0, 0, 0);
    }

    public void RotateBuilding()
    {
        if (Input.GetKeyDown(rotateBuildingLeft))
        {
            rotateAngle -= 15;
        }
        else if (Input.GetKeyDown(rotateBuildingRight))
        {
            rotateAngle += 15;
        }
        outline.transform.rotation = Quaternion.Euler(0, rotateAngle, 0);
        angleToSpawnTower = Quaternion.Euler(0, rotateAngle, 0);
    }

    public void DeSelect()
    {
         
        if (GameMenu.Instance.isPaused)
        {
            shouldOutline = false;
            return;
        }
        // Stops user from placing when clicking a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            shouldOutline = false;
            rotateAngle = 0;
            StopAllCoroutines();
            return;
        }

        if (Input.GetKey(spawnMultiple) && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(SpawnTowerRoutine());
            return;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            SpawnTowerAtMouse();
        }
        shouldOutline = false;
    }

    public void Selected()
    {
        
        if (GameMenu.Instance.isPaused)
        {
            return;
        }
        SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 1f);
        button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        string towerName = button.name.Replace("Btn", "");

        TextMeshProUGUI[] buttonTexts = button.GetComponentsInChildren<TextMeshProUGUI>();

        priceOfObject = int.Parse(buttonTexts[1].text.Replace("Costs: ", ""));
        tower = Resources.Load<GameObject>("Prefabs/Buildings/" + towerName);

        shouldOutline = true;
        // setting the size of outline a little bigger than tower
        Vector3 outlineSize = new Vector3(tower.transform.localScale.x + 0.4f, 0.01f, tower.transform.localScale.z + 0.4f);
        outline.transform.localScale = outlineSize;
        StartCoroutine(TowerOutline());
    }

    public void SelectedDeleteBtn()
    {
        SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 1f);
        shouldOutline = false;
        shouldOutlineDelete = true;
        StartCoroutine(HighlightTower());
    }

    public void DeSelectDeleteBtn()
    {
        shouldOutlineDelete = false;
        if (!Input.GetKeyDown(KeyCode.Mouse0))
        {
            return;
        }

        Ray ray = ComponentManager.Instance.buildCam.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Kingdom"))
            {
                ComponentManager.Instance.CallCoroutine(ComponentManager.Instance.ShowMessage("You Can`t Delete The Kingdom! >:("));
                return;
            }

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("whatIsBuilding"))
            {
                if (hit.collider.TryGetComponent(out Building building))
                {
                    CurrencyManager.Instance.Currency += building.GetMoneySpent();
                }
                else if (hit.collider.transform.parent.parent.TryGetComponent(out Building building1))
                {
                    CurrencyManager.Instance.Currency += building1.GetMoneySpent();
                }

                Destroy(hit.transform.gameObject);
            }
        }
    }

    private IEnumerator HighlightTower()
    {
        Material hitMaterial = default;
        Renderer rendersder = default;

        while (shouldOutlineDelete && !Input.GetKeyDown(cancel))
        {
            // Deselects the button when user pauses the game
            if (Input.GetKeyDown(GameMenu.Instance.pauseGame))
            {
                EventSystem.current.SetSelectedGameObject(null);
                break;
            }

            Ray ray = ComponentManager.Instance.buildCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("whatIsBuilding"))
                {
                    if (rendersder == null || rendersder.gameObject != hit.collider.gameObject)
                    {
                        if (rendersder != null && hitMaterial != null)
                        {
                            rendersder.sharedMaterial = hitMaterial;
                        }

                        rendersder = hit.collider.gameObject.GetComponentInChildren<Renderer>();
                        hitMaterial = rendersder.sharedMaterial;

                        rendersder.sharedMaterial = materiasdal;
                    }
                }
                // If its not whatIsBuilding and hitMaterial not default
                else if (hitMaterial != default)
                {
                    // Sets it back to original and empties variable
                    rendersder.sharedMaterial = hitMaterial;
                    hitMaterial = default;
                    rendersder = default;
                }
            }
            yield return null;
        }

        if (hitMaterial != default && rendersder != null)
        {
            rendersder.sharedMaterial = hitMaterial;
        }
    }

    bool IsThereAnyColliders()
    {
        // The area it checks
        Vector3 pos = new Vector3(outline.transform.position.x, outline.transform.position.y + 0.5f, outline.transform.position.z);

        Collider[] colliders = Physics.OverlapBox(pos, outline.transform.localScale / 2, outline.transform.rotation);
        return colliders.Length != 0;
    }

    bool IsItInContactWithGround()
    {
        Vector3 pos = new Vector3(outline.transform.position.x, outline.transform.position.y, outline.transform.position.z);

        Collider[] colliders = Physics.OverlapBox(pos, outline.transform.localScale / 2, outline.transform.rotation, LayerMask.GetMask("whatIsGround"));
        return colliders.Length != 0;
    }

    bool IsCorrectArea()
    {
        return IsItInContactWithGround() && !IsThereAnyColliders();
    }

    private void HighlightButton(bool isActive, Button buttonToChange, Color color = default)
    {
        if (color == default)
        {
            color = Color.green;
        }

        ColorBlock colors = buttonToChange.colors;
        colors.normalColor = isActive ? color : Color.white;
        buttonToChange.colors = colors;
    }

    private void SpawnTowerAtMouse()
    {
        Ray ray = ComponentManager.Instance.buildCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!IsCorrectArea())
        {
            ComponentManager.Instance.CallCoroutine(ComponentManager.Instance.ShowMessage("You Can't Place There"));
            ComponentManager.Instance.CallCoroutine(FlashButton(Color.yellow));
            return;
        }

        if (CurrencyManager.Instance.Currency >= priceOfObject)
        {
            if (Physics.Raycast(ray, out hit))
            {
                SoundFXManager.instance.PrepareSoundFXClip(audioPath2, transform, 0.5f);
                Vector3 spawnLocation = new Vector3(hit.point.x, hit.point.y + 2.7f, hit.point.z);

                Instantiate(tower, spawnLocation, angleToSpawnTower);
                CurrencyManager.Instance.Currency -= priceOfObject;
            }
        }
        else
        {
            costRemaining = priceOfObject - CurrencyManager.Instance.Currency;
            ComponentManager.Instance.CallCoroutine(ComponentManager.Instance.ShowMessage("You need " + costRemaining + "$ more"));
            ComponentManager.Instance.CallCoroutine(FlashButton(Color.red));
        }
    }

    private IEnumerator FlashButton(Color color)
    {
        Button saveButt = button;
        HighlightButton(true, saveButt, color);
        yield return new WaitForSeconds(0.3f);
        HighlightButton(false, saveButt, color);
        yield return new WaitForSeconds(0.3f);
        HighlightButton(true, saveButt, color);
        yield return new WaitForSeconds(0.3f);
        HighlightButton(false, saveButt, color);
    }
}
