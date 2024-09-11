using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBalls : MonoBehaviour
{
    public GameObject projectile;
    public KeyCode SpawnKey = KeyCode.M;
    public float spawnSpeed = 0.01f;
    float timer = 0.0f;

    void Update()
    {
        Ball();
    }

    private string Ball() 
    {
        if (Time.time > timer)
        {
            if (Input.GetKey(SpawnKey))
            {
                _ = Instantiate(projectile, transform.position, transform.rotation);
            }
            timer += spawnSpeed;
        }
        return "";
    }
}
