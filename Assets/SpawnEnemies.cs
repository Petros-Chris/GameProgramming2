using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class SpawnEnemies : MonoBehaviour
{
    public GameObject Enemy;
    public KeyCode SpawnKey = KeyCode.E;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(SpawnKey))
        {
            GameObject enemy = Instantiate(Enemy, transform.position, transform.rotation);
        }
    }
}
