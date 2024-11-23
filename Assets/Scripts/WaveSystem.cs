using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveSystem : MonoBehaviour
{
    public LayerMask EnemyLayer;
    bool currentlyInIntermission = false;
    bool displaySlider = false;
    float timer;
    public Slider skipIntermissionSlider;
    Coroutine intermissionCoroutine;
    Coroutine waveCoroutine;
    Coroutine skipCurrentRountCoroutine;
    // TODO: Automatically get all spawn points
    public GameObject[] spawnPoints;
    public float spawnRate = 3.0f;
    public KeyCode skipIntermissionKey = KeyCode.L;
    bool allEnemiesSpawned;
    bool waveInProgress;

    void Start()
    {

    }

    void Update()
    {
        // If round is not currently in intermission
        if (!currentlyInIntermission)
        {
            // If no enemies are left and the round is not in progess
            if (!EnemiesLeft() && !waveInProgress)
            {
                // Stop the coroutine if still counting down
                if (waveCoroutine != null)
                {
                    StopCoroutine(waveCoroutine);
                    waveCoroutine = null; //Because I don't think StopCoroutine makes it null again
                }

                if (skipCurrentRountCoroutine != null)
                {
                    StopCoroutine(skipCurrentRountCoroutine);
                    skipCurrentRountCoroutine = null;
                }

                intermissionCoroutine = StartCoroutine(BeginIntermissionToNextWave());
                if (ComponentManager.playerCam.gameObject.activeSelf)
                {
                    StartCoroutine(DisplayIntermissionSlider(skipIntermissionSlider));
                }
            }

            // If all enemies have spawned and coroutine is not currently running
            if (allEnemiesSpawned && waveCoroutine == null)
            {
                waveCoroutine = StartCoroutine(BeginCountdownToDisplaySlider());
            }
        }
    }

    IEnumerator DisplayIntermissionSlider(Slider slider)
    {
        slider.gameObject.SetActive(true);
        while (displaySlider)
        {
            while (Input.GetKey(skipIntermissionKey))
            {
                timer += Time.deltaTime;
                slider.value = timer;
                if (timer >= 4)
                {
                    BeginWave();
                    StopCoroutine(intermissionCoroutine);
                    timer = 0;
                    slider.value = 0;
                    currentlyInIntermission = false;
                    break;
                }
                yield return null;
            }

            while (!Input.GetKey(skipIntermissionKey) && timer >= 0)
            {
                timer -= Time.deltaTime;
                slider.value = timer;
                yield return null;
            }
            yield return null;
        }
        timer = 0;
        slider.value = timer;
        slider.gameObject.SetActive(false);
    }

    public void BeginWave()
    {
        waveInProgress = true;
        displaySlider = false;
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        float timer = 0;
        int enemiesSpawned = 0;
        //TODO: Figure out how to get how many enemies should spawn (json file?)
        while (enemiesSpawned < 5)
        {
            timer += Time.deltaTime;
            if (timer >= spawnRate)
            {
                // Gets position to spawn at
                int pointToSpawn = Random.Range(0, spawnPoints.Length);
                Vector3 pos = spawnPoints[pointToSpawn].transform.position;
                // TODO: Figure out how many of one type of enemy should spawn (json file?)
                Instantiate(ComponentManager.defaultEnemy, pos, Quaternion.identity);
                enemiesSpawned++;
                timer = 0;
            }
            yield return null;
        }
        allEnemiesSpawned = true;
        waveInProgress = false;
        waveCoroutine = null;
    }

    IEnumerator BeginIntermissionToNextWave()
    {
        Debug.Log("Intermission!");
        currentlyInIntermission = true;
        displaySlider = true;
        yield return new WaitForSeconds(20);
        Debug.Log("Next Round About To Start!");
        yield return new WaitForSeconds(10);
        Debug.Log("Round Starting!");
        currentlyInIntermission = false;
        displaySlider = false;
        BeginWave();
    }

    public bool EnemiesLeft()
    {
        Collider[] EnemiesAlive = Physics.OverlapSphere(transform.position, 10000, EnemyLayer);

        if (EnemiesAlive.Length == 0)
        {
            allEnemiesSpawned = false;
            return false;
        }
        return true;
    }

    IEnumerator BeginCountdownToDisplaySlider()
    {
        Debug.Log("Beginning Skip!");
        yield return new WaitForSeconds(10);
        Debug.Log("Displaying Skip!");
        displaySlider = true;
        skipCurrentRountCoroutine = StartCoroutine(DisplayIntermissionSlider(skipIntermissionSlider));
    }
}