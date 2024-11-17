using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class BuildController : MonoBehaviour
{
    //!BUG: if game lose while player in build mode, player frozen next game start
    //!BUG: Can place ontop of object
    //!BUG: Will place new building if select building button

    public GameObject tower;
    public KeyCode spawnMultiple = KeyCode.RightShift;
    public KeyCode cancel = KeyCode.C;
    private Button button;
    public GameObject outline;
    private bool shouldOutline;
    Renderer render;
    int rotateAngle;
    Quaternion angleToSpawnTower;

    //public NavMeshSurface navMeshSurface;
    void Start()
    {
        render = outline.GetComponent<Renderer>();
    }

    private IEnumerator SpawnTowerRoutine()
    {
        while (Input.GetKey(spawnMultiple))
        {
            if (Input.GetMouseButtonDown(0))
            {
                SpawnTowerAtMouse();
            }
            // Changes color to show you are in multi place mode
            HighlightButton(true);
            yield return null;
        }
        shouldOutline = false;
        HighlightButton(false);
    }
    //TODO: R for rotate 
    private IEnumerator TowerOutline()
    {
        while (shouldOutline && !Input.GetKeyDown(cancel))
        {
            Ray ray = ComponentManager.buildCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Always at same level, issue for terrain?
                outline.transform.position = hit.point;
                //outline.transform.position = new Vector3(hit.point.x, 0.01f, hit.point.z);
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                rotateAngle += 90;
                outline.transform.rotation = Quaternion.Euler(0, rotateAngle, 0);
                angleToSpawnTower = Quaternion.Euler(0, rotateAngle, 0);
            }

            if (!IsThereAnyColliders())
            {
                render.material.color = Color.white;
            }
            else
            {
                render.material.color = Color.green;
            }
            yield return null;
        }
        // Setting the outline out of bounds
        outline.transform.position = new Vector3(0, -1000, 0);

        // If you want it to always go back to default angle
        rotateAngle = 0;
        outline.transform.rotation = Quaternion.Euler(0, 0, 0);
        angleToSpawnTower = Quaternion.Euler(0, 0, 0);
    }

    public void DeSelect()
    {
        if (GameMenu.isPaused)
        {
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
        if (GameMenu.isPaused)
        {
            return;
        }
        button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        string towerName = button.name.Replace("Btn", "");
        tower = Resources.Load<GameObject>("Prefabs/Buildings/" + towerName);

        shouldOutline = true;
        // setting the size of outline a little bigger than tower
        Vector3 outlineSize = new Vector3(tower.transform.localScale.x + 1, 0.01f, tower.transform.localScale.z + 1);
        outline.transform.localScale = outlineSize;
        StartCoroutine(TowerOutline());
    }

    bool IsThereAnyColliders()
    {
        // The area it checks
        Vector3 pos = new Vector3(outline.transform.position.x, outline.transform.position.y + 0.5f, outline.transform.position.z);

        Collider[] colliders = Physics.OverlapBox(pos, outline.transform.localScale / 2, Quaternion.identity);

        if (colliders.Length != 0)
        {
            return true;
        }
        return false;
    }

    private void HighlightButton(bool isActive)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = isActive ? Color.green : Color.white;
        button.colors = colors;
    }

    // Will not update the enemy table on new towers, is that bad? 
    // (depends on if were allowing the player to build mid battle, maybe we could but there would be a timer of like 30 seconds?)
    private void SpawnTowerAtMouse()
    {
        Ray ray = ComponentManager.buildCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (IsThereAnyColliders())
        {
            return;
        }

        //TODO: Stop place if no money?
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 spawnLocation = new Vector3(hit.point.x, tower.transform.position.y, hit.point.z);

            Instantiate(tower, spawnLocation, angleToSpawnTower);
            // A bit resource intensive, likely worse for larger maps
            //navMeshSurface.BuildNavMesh();
        }
    }
}
