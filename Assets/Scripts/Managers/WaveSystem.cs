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
    public TextMeshProUGUI skipSliderText;
    Coroutine intermissionCoroutine;
    Coroutine waveCoroutine;
    Coroutine skipCurrentRoundCoroutine;
    // TODO: Automatically get all spawn points
    public GameObject[] spawnPoints;
    public float spawnRate = 3.0f;
    public KeyCode skipIntermissionKey = KeyCode.L;
    bool allEnemiesSpawned;
    bool waveInProgress;
    int round = 0;
    int wavesToComplete;

    public TextMeshProUGUI waveText;
    public TextMeshProUGUI EnemyOrTimerText;

    bool isLastRound = false;
    JsonHandler.Root root;
    int intermissionTimer;

    //!BUG: If holding slider key when round starts, slider doesn't dissaper immediately
    void Start()
    {
        root = JsonHandler.ReadFileForWave("waves");
        Debug.Log(root.difficulty[0].difficulty);
        wavesToComplete = root.difficulty[0].waves.Count;
        waveText.text = "Wave: " + 0 + "/" + wavesToComplete;
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
                ComponentManager.Instance.lockCamera = false;
                // Stop the coroutine if still counting down
                if (waveCoroutine != null)
                {
                    StopCoroutine(waveCoroutine);
                    waveCoroutine = null; //Because I don't think StopCoroutine makes it null again
                }

                if (skipCurrentRoundCoroutine != null)
                {
                    StopCoroutine(skipCurrentRoundCoroutine);
                    skipCurrentRoundCoroutine = null;
                }

                RewardMoney(0);

                intermissionCoroutine = StartCoroutine(BeginIntermissionToNextWave());
                if (ComponentManager.Instance.playerCam.gameObject.activeSelf)
                {
                    StartCoroutine(DisplayIntermissionSlider(skipIntermissionSlider, "Hold L To Skip Intermission"));
                }
            }
            if (!EnemiesLeft() && !waveInProgress && isLastRound)
            {
                ComponentManager.Instance.lockCamera = false; // Remove when scene changes as its just for now 
                ComponentManager.Instance.DisplayWinScreen();
            }

            // If all enemies have spawned and coroutine is not currently running
            if (allEnemiesSpawned && waveCoroutine == null)
            {
                waveCoroutine = StartCoroutine(BeginCountdownToDisplaySlider());
            }
        }
    }

    IEnumerator DisplayIntermissionSlider(Slider slider, string message)
    {
        if (slider == null)
        {
            yield break;
        }
        slider.gameObject.SetActive(true);
        skipSliderText.text = message;
        while (displaySlider && !isLastRound)
        {
            while (Input.GetKey(skipIntermissionKey))
            {
                timer += Time.deltaTime;
                slider.value = timer;
                if (timer >= 4)
                {
                    BeginWave();
                    RewardMoney(intermissionTimer);
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
        ComponentManager.Instance.SwitchToPlayerAndLockCamera();
        ComponentManager.Instance.CallCoroutine(ComponentManager.Instance.ShowMessage("Round Starting!"));
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
        waveText.text = "Wave: " + waveNumber + "/" + wavesToComplete;
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
                        Instantiate(ComponentManager.Instance.defaultEnemy, pos, Quaternion.identity);
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
        intermissionTimer = 30;
        Debug.Log("Intermission!");
        currentlyInIntermission = true;
        displaySlider = true;
        while (intermissionTimer > 0)
        {
            yield return new WaitForSeconds(1f);
            EnemyOrTimerText.text = "Time Until Next Round " + --intermissionTimer;
        }
        EnemyOrTimerText.text = "Round Starting!";
        currentlyInIntermission = false;
        displaySlider = false;
        BeginWave();
    }

    public bool EnemiesLeft()
    {
        Collider[] EnemiesAlive = Physics.OverlapSphere(transform.position, 10000, EnemyLayer);
        EnemyOrTimerText.text = "Enemies Left " + EnemiesAlive.Length;

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
        skipCurrentRoundCoroutine = StartCoroutine(DisplayIntermissionSlider(skipIntermissionSlider, "Hold L To Begin Next Wave"));
    }

    private void RewardMoney(int timeSkipped = default)
    {
        int reward = 0;

        if (timeSkipped != default)
        {
            reward += round * (timeSkipped / 10);
        }
        reward += round * 50;

        CurrencyManager.Instance.Currency += reward;
    }
    //GUI for showing base health
    //!BUG: Skip round in deathcam, casuing broken cam when suceesufly skiiped

    //GUI for showing boss health (when he exists)
}