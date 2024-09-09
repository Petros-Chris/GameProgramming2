using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pissscript : MonoBehaviour
{
    public GameObject projectile;
    public KeyCode SpawnKey = KeyCode.P;

    void Update()
    {
        //InvokeRepeating(Ball(), 2.0f, 0.5f);
        Ball();
    }
    private string Ball()
    {
        if (Input.GetKey(SpawnKey))
        {
            GameObject ball = Instantiate(projectile, transform.position, transform.rotation);
        }
        return "";
    }
}
