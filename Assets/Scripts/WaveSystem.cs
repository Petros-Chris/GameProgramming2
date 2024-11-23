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
    int round = 0;
    int wavesToComplete;

    bool isLastRound = false;
    JsonHandler.Root root;

    void Start()
    {
        root = JsonHandler.ReadFileForWave("waves");
        Debug.Log(root.difficulty[0].difficulty);
        wavesToComplete = root.difficulty[0].waves.Count;
        // foreach (var difficulty in root.difficulty)
        // {
        //     print("difficulty Level: " + difficulty.difficulty);
        //     foreach (var wave in difficulty.waves)
        //     {
        //         print("wave Num: " + wave.wave);
        //         print("anmount of enemmiwas: " + wave.totalEnemies);
        //         foreach (var enemy in wave.enemies)
        //         {
        //             print("count of type: " + enemy.count + " type of enemiesas: " + enemy.type);
        //         }
        //     }
        // }
    }

    void Update()
    {
        // If round is not currently in intermission
        if (!currentlyInIntermission)
        {
            if (wavesToComplete <= round)
            {
                isLastRound = true;
            }

            // If no enemies are left and the round is not in progess
            if (!EnemiesLeft() && !waveInProgress && !isLastRound)
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
            if (!EnemiesLeft() && !waveInProgress && isLastRound)
            {
                //Switch scene
                Debug.Log("YOU WIN");
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
        while (displaySlider && !isLastRound)
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
        round++;
        StartCoroutine(SpawnWave());

    }

    IEnumerator SpawnWave()
    {
        float timer = 0;
        int enemiesSpawned = 0;
        int countBasic = 0;

        var easyMode = root.difficulty[0];
        var wave = easyMode.waves[round - 1];
        var waveNumber = wave.wave;
        var totalEnemiesInRound = wave.totalEnemies;

        while (enemiesSpawned < totalEnemiesInRound)
        {
            timer += Time.deltaTime;
            if (timer >= spawnRate)
            {
                // Gets position to spawn at
                int pointToSpawn = Random.Range(0, spawnPoints.Length);
                Vector3 pos = spawnPoints[pointToSpawn].transform.position;

                if (wave.enemies[0].type == "basic")
                {
                    if (wave.enemies[0].count > countBasic)
                    {
                        Instantiate(ComponentManager.defaultEnemy, pos, Quaternion.identity);
                    }
                    countBasic++;
                }
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