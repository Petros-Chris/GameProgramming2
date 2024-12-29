using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private GameObject enemyToCreate;
    private GameObject playerToCreate;
    public Vector3 wherePlayerSpawn = new Vector3(52, 0.9f, 22);
    JsonHandler.DifficultyLevel difficultyLevel;
    int worldLevelInt;
    bool winDisplayed;
    public string audioPath = "CountDownStartRound";

    public string audioPath3 = "StartRound";
    public string audioPath4 = "RoundEnd";
    public string audioPath5 = "BuildingRespawn";

    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string worldLevelStr = currentScene.name.Replace("Level", "");
        root = JsonHandler.ReadFileForWave("waves");
        worldLevelInt = int.Parse(worldLevelStr) - 1; // Remember that it is a array so it starts at 0 instead of 1
        difficultyLevel = root.level[worldLevelInt].difficulty[DifficultyHandler.Instance.ReturnDifficultyInInt()];
        wavesToComplete = difficultyLevel.waves.Count;
        waveText.text = "Wave: " + 0 + "/" + wavesToComplete;

        SaveScene.DataToSave playerData = SaveScene.ReadSaveFile();
        // Stops it from moving the scene save down because player restarts after beating level 
        if (playerData.worldLevel < SceneManager.GetActiveScene().buildIndex)
        {
            SaveScene.Save(SceneManager.GetActiveScene().buildIndex, DifficultyHandler.Instance.ReturnDifficultyInInt());
        }
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

                if (SoundFXManager.instance.globalAudioObject != default)
                    SoundFXManager.instance.DestroyGlobalSound(SoundFXManager.instance.globalAudioObject);
                if (round != 0)
                {
                    SoundFXManager.instance.PrepareSoundFXClip(audioPath4, transform, 0.5f, true, false);
                }
                // Allows the player to switch into build mode again
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
                ReviveAllTowers();
                RevivePlayer();

                intermissionCoroutine = StartCoroutine(BeginIntermissionToNextWave());
                if (ComponentManager.Instance.playerCam.gameObject.activeSelf)
                {
                    StartCoroutine(DisplayWaveSlider(skipIntermissionSlider, "Hold L To Skip Intermission"));
                }
            }

            if (!EnemiesLeft() && !waveInProgress && isLastRound && !winDisplayed)
            {
                if (SoundFXManager.instance.globalAudioObject != default)
                    SoundFXManager.instance.DestroyGlobalSound(SoundFXManager.instance.globalAudioObject);

                ComponentManager.Instance.lockCamera = false;
                ComponentManager.Instance.DisplayWinScreen();
                SaveScene.Save(SceneManager.GetActiveScene().buildIndex + 1, DifficultyHandler.Instance.ReturnDifficultyInInt());
                winDisplayed = true;
            }

            // If all enemies have spawned and coroutine is not currently running
            if (allEnemiesSpawned && waveCoroutine == null)
            {
                waveCoroutine = StartCoroutine(BeginCountdownToDisplaySlider());
            }
        }
    }

    //? Can spawn on top of player, but it doesn't seem to push them out of map so perhaps its fine
    public void ReviveAllTowers()
    {
        if (SoundFXManager.instance != null && ComponentManager.Instance.buildingsDisabled.Count != 0)
            SoundFXManager.instance.PrepareSoundFXClip(audioPath5, transform, 0.5f, true);
        foreach (GameObject building in ComponentManager.Instance.buildingsDisabled)
        {
            if (building == null)
            {
                // if game object was deleted
                continue;
            }
            building.SetActive(true);
            // Giving object health 
            Building buildingScript = building.GetComponent<Building>();
            buildingScript.health = 50; // towerScript.maxHealth if you want it to come back with max health
            buildingScript.GetHealthBar().UpdateHealthBar(buildingScript.health, buildingScript.maxHealth);
        }
        // So it won't attempt to fix the same buildings next round
        ComponentManager.Instance.buildingsDisabled.Clear();
    }

    public void RevivePlayer()
    {
        if (ComponentManager.Instance.hasPlayerDied)
        {

            Debug.Log("HI");
            playerToCreate = Resources.Load<GameObject>("Prefabs/Characters/Player");

            Instantiate(playerToCreate, wherePlayerSpawn, Quaternion.identity);
            // ComponentManager.Instance.ReAssignCameras();
            ComponentManager.Instance.SwitchToPlayerAndLockCamera(false); // Should probably make another method for this
        }
    }

    IEnumerator DisplayWaveSlider(Slider slider, string message)
    {
        if (slider == null)
        {
            yield break;
        }
        slider.gameObject.SetActive(true);
        skipSliderText.text = message;
        while (displaySlider && !isLastRound)
        {
            // while the key is pressed and the slider is meant to be shown
            while (Input.GetKey(skipIntermissionKey) && displaySlider)
            {
                timer += Time.deltaTime;
                slider.value = timer;
                if (timer >= 2)
                {
                    BeginWave();
                    RewardMoney(intermissionTimer);
                    RevivePlayer(); // If player starts next round while dead, this makes them come back
                    StopCoroutine(intermissionCoroutine);
                    timer = 0;
                    slider.value = 0;
                    currentlyInIntermission = false;
                    break;
                }
                yield return null;
            }
            // while the key is pressed, the value is not 0, and the slider is meant to be shown
            while (!Input.GetKey(skipIntermissionKey) && timer >= 0 && displaySlider)
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
        StartCoroutine(DestroyAllGlobalAudio());
        StartCoroutine(SpawnWave());
    }

    public IEnumerator DestroyAllGlobalAudio()
    {
        while (SoundFXManager.instance.globalAudioObject)
        {
            SoundFXManager.instance.DestroyGlobalSound(SoundFXManager.instance.globalAudioObject);
            yield return null;
        }
        SoundFXManager.instance.PrepareSoundFXClip("StartRound", transform, 1f, true);
        SoundFXManager.instance.PrepareSoundFXClip("inGameSound", transform, 1f, true, true);
    }

    IEnumerator SpawnWave()
    {
        int enemiesSpawned = 0;
        int count = 0;
        float timer2 = 0;

        var wave = difficultyLevel.waves[round - 1];
        var waveNumber = wave.wave;
        waveText.text = "Wave: " + waveNumber + "/" + wavesToComplete;
        var totalEnemiesInRound = wave.totalEnemies;

        while (enemiesSpawned < totalEnemiesInRound)
        {
            foreach (var enemy in wave.enemies)
            {
                enemyToCreate = Resources.Load<GameObject>("Prefabs/Characters/Enemies/" + enemy.type + "Enemy");
                while (count < enemy.count)
                {
                    timer2 += Time.deltaTime;
                    if (timer2 >= spawnRate)
                    {
                        // Gets position to spawn at
                        int pointToSpawn = Random.Range(0, spawnPoints.Length);
                        Vector3 pos = spawnPoints[pointToSpawn].transform.position;

                        Instantiate(enemyToCreate, pos, Quaternion.identity);
                        enemiesSpawned++;
                        timer2 = 0;
                        count++;
                    }
                    yield return null;
                }
                count = 0;
            }
        }
        allEnemiesSpawned = true;
        waveInProgress = false;
        waveCoroutine = null;
    }

    IEnumerator BeginIntermissionToNextWave()
    {

        intermissionTimer = 90;
        Debug.Log("Intermission!");
        currentlyInIntermission = true;
        displaySlider = true;
        while (intermissionTimer > 0)
        {
            yield return new WaitForSeconds(1f);
            EnemyOrTimerText.text = "Time Until Next Round " + --intermissionTimer;
            if (intermissionTimer == 6)
            {
                SoundFXManager.instance.PrepareSoundFXClip(audioPath, transform, 0.5f, true);
            }
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
        skipCurrentRoundCoroutine = StartCoroutine(DisplayWaveSlider(skipIntermissionSlider, "Hold L To Begin Next Wave"));
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