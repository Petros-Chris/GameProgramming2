using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public GameObject defaultEnemy;
    public GameObject fastEnemy;
    public GameObject tankEnemy;
    private KeyCode spawnKey = KeyCode.J;
    private KeyCode spawnKeyStrong = KeyCode.K;
    private KeyCode spawnKeyFast = KeyCode.L;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            Instantiate(defaultEnemy, transform.position, transform.rotation);
        }
        if (Input.GetKeyDown(spawnKeyFast))
        {
            Instantiate(fastEnemy, transform.position, transform.rotation);

        }
        if (Input.GetKeyDown(spawnKeyStrong))
        {
            Instantiate(tankEnemy, transform.position, transform.rotation);

        }
    }
}
