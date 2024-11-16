using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    public LayerMask EnemyLayer;
    bool wait = false;

    void Start()
    {

    }

    void Update()
    {
        // To make sure this aint happening twice
        if (!wait)
        {
            //? Or maybe we combine all of them to have a auto timer for wave, a button if you don't want to wait, and a key if you don't want to press the button via mouse
            if (!EnemiesLeft())
            {
                StartCoroutine(IntermissionToNextWave());
            }
            // If we want to press a key to start the next round
            if (Input.GetKeyDown(KeyCode.P))
            {

            }
        }
    }

    // If we want to press a gui to start next wave
    // Gui should only appear when current round has ended
    public void StartNextWave()
    {
        // For Button...
        if (Input.GetMouseButtonDown(0))
        {
            BeginWave();
        }
    }

    public void BeginWave()
    {
        // By adjusting this, you could make them spawn different areas
        // A better way though would be to get all spawnpoints as gameobjects
        // So it could easily be adjusted and changed
        // just have a math.random function to make enemies spawn at different spawnpoints nicely
        Vector3 pos = new Vector3(-20, 0.5f, 0);

        Instantiate(ComponentManager.defaultEnemy, pos, Quaternion.identity);
        Instantiate(ComponentManager.tankEnemy, pos, Quaternion.identity);
        Instantiate(ComponentManager.fastEnemy, pos, Quaternion.identity);
    }

    // If were doing a timer setup
    public IEnumerator IntermissionToNextWave()
    {
        Debug.Log("Intermission!");
        wait = true;
        yield return new WaitForSeconds(20);
        Debug.Log("Next Round About To Start!");
        yield return new WaitForSeconds(10);
        Debug.Log("Round Starting!");
        wait = false;
        BeginWave();
    }

    public bool EnemiesLeft()
    {
        Collider[] EnemiesAlive = Physics.OverlapSphere(transform.position, 10000, EnemyLayer);

        if (EnemiesAlive.Length == 0)
        {
            return false;
        }
        return true;
    }

    // Or maybe it should be a timer where if 1 minute has passed to show the btn
    public void EnemiesLeftForGui()
    {
        Collider[] EnemiesAlive = Physics.OverlapSphere(transform.position, 10000, EnemyLayer);

        if (EnemiesAlive.Length <= 4)
        {
            // startNextWaveBtn.setActive(true);
            //return false;
        }
        //return true;
    }

    // If we want a timer to show it instead
    public IEnumerator ShowNextWaveBtn()
    {
        yield return new WaitForSeconds(60);
        // startNextWaveBtn.setActive(true);

    }
}