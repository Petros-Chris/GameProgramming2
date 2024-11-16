using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class BuildController : MonoBehaviour
{
    public GameObject tower;
    public KeyCode spawnMultiple = KeyCode.RightShift;
    private Button button;

    public GameObject outline;

    private bool shouldOutline;
    //public NavMeshSurface navMeshSurface;

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
        HighlightButton(false);
    }

    private IEnumerator TowerOutline()
    {
        while (shouldOutline && !Input.GetKeyDown(KeyCode.Q))//Cancel Key
        {
            Ray ray = ComponentManager.buildCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                outline.transform.position = hit.point;
            }

            yield return null;
        }
        // Setting the outline out of bounds
        outline.transform.position = new Vector3(0, -1000, 0);
    }

    public void Selected()
    {
        button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        string towerName = button.name.Replace("Btn", "");
        tower = Resources.Load<GameObject>("Prefabs/Buildings/" + towerName);

        shouldOutline = true;
        // setting the size of outline a little bigger than tower
        Vector3 outlineSize = new Vector3(tower.transform.localScale.x + 1, 0.01f, tower.transform.localScale.z + 1);
        outline.transform.localScale = outlineSize;
        StartCoroutine(TowerOutline());
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

        //TODO: Need to stop tower from being placed anywhere
        //TODO: Stop place if no money?
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 spawnLocation = new Vector3(hit.point.x, hit.point.y + 2.5f, hit.point.z);

            Instantiate(tower, spawnLocation, Quaternion.identity);
            // A bit resource intensive, likely more for larger maps
            //navMeshSurface.BuildNavMesh();
        }
    }

    public void DeSelect()
    {
        if (Input.GetKey(spawnMultiple) && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(SpawnTowerRoutine());
        }
        else if (Input.GetMouseButtonDown(0))
        {
            SpawnTowerAtMouse();
        }
        shouldOutline = false;
    }
}
