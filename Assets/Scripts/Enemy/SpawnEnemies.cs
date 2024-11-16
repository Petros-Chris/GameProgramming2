using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public GameObject defaultEnemy;
    public GameObject fastEnemy;
    public GameObject tankEnemy;
    public KeyCode SpawnKey = KeyCode.E;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(SpawnKey))
        {
            for (int i = 0; i < 5; i++)
            {
                Instantiate(defaultEnemy, transform.position, transform.rotation);
                Instantiate(fastEnemy, transform.position, transform.rotation);
                Instantiate(fastEnemy, transform.position, transform.rotation);
            }

        }
    }
}
