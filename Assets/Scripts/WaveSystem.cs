using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WaveSystem : MonoBehaviour
{
    public LayerMask EnemyLayer;
    bool wait = false;
    float timer;
    public Slider skipIntermissionSlider;
    Coroutine intermissionCoroutine;
    Coroutine waveCoroutine;

    bool allEnemiesSpawned;

    void Start()
    {

    }

    void Update()
    {
        if (allEnemiesSpawned && waveCoroutine == null)
        {
            //waveCoroutine = StartCoroutine(ShowNextWaveSlider());
        }

        // To make sure this aint happening twice
        if (!wait)
        {
            if (!EnemiesLeft())
            {
                if (waveCoroutine != null)
                {
                    StopCoroutine(waveCoroutine); // As round has already ended, not needed anymore
                }

                intermissionCoroutine = StartCoroutine(IntermissionToNextWave());
                if (ComponentManager.playerCam.gameObject.activeSelf)
                {
                    StartCoroutine(DisplayIntermissionSlider());
                }
            }
        }
    }

    private IEnumerator DisplayIntermissionSlider()
    {
        skipIntermissionSlider.gameObject.SetActive(true);
        while (wait)
        {
            while (Input.GetKey(KeyCode.L))
            {
                timer += Time.deltaTime;
                skipIntermissionSlider.value = timer;
                if (timer >= 4)
                {
                    BeginWave();
                    StopCoroutine(intermissionCoroutine);
                    timer = 0;
                    skipIntermissionSlider.value = 0;
                    wait = false;

                    break;
                }
                yield return null;
            }

            while (!Input.GetKey(KeyCode.L) && timer >= 0)
            {
                timer -= Time.deltaTime;
                skipIntermissionSlider.value = timer;
                yield return null;
            }
            yield return null;
        }
        timer = 0;
        skipIntermissionSlider.value = timer;
        skipIntermissionSlider.gameObject.SetActive(false);
    }

    public void BeginWave()
    {
        // By adjusting this, you could make them spawn different areas
        // A better way though would be to get all spawnpoints as gameobjects
        // So it could easily be adjusted and changed
        // just have a math.random function to make enemies spawn at different spawnpoints nicely
        Vector3 pos = new Vector3(-20, 0.5f, 0);

        // TODO: add a delay for each spawn to prevent massive rush
        Instantiate(ComponentManager.defaultEnemy, pos, Quaternion.identity);
        Instantiate(ComponentManager.tankEnemy, pos, Quaternion.identity);
        Instantiate(ComponentManager.fastEnemy, pos, Quaternion.identity);

        allEnemiesSpawned = true;
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
            allEnemiesSpawned = false;
            return false;
        }
        return true;
    }

    public IEnumerator ShowNextWaveSlider()
    {
        yield return new WaitForSeconds(10);
        // Maybe we should have it more obvious so the player
        // won't accidentally skip intermission thinking it was just skipping to intermission?
        //like skipWaveAndIntermissionSlider.gameObject.SetActive(true);

        skipIntermissionSlider.gameObject.SetActive(true);
        while (allEnemiesSpawned)
        {
            while (Input.GetKey(KeyCode.L))
            {
                timer += Time.deltaTime;
                skipIntermissionSlider.value = timer;
                if (timer >= 4)
                {
                    BeginWave();
                    StopCoroutine(intermissionCoroutine);
                    timer = 0;
                    skipIntermissionSlider.value = 0;
                    wait = false;

                    break;
                }
                yield return null;
            }

            while (!Input.GetKey(KeyCode.L) && timer >= 0)
            {
                timer -= Time.deltaTime;
                skipIntermissionSlider.value = timer;
                yield return null;
            }
            yield return null;
        }
        timer = 0;
        skipIntermissionSlider.value = timer;
        skipIntermissionSlider.gameObject.SetActive(false);
    }
}