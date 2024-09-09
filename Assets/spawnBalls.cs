using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnBalls : MonoBehaviour
{
    public GameObject projectile;
    public KeyCode SpawnKey = KeyCode.M;
    bool flag = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InvokeRepeating(Ball(), 2.0f, 0.5f);
                
    }
    private string Ball() 
    {
        if (Input.GetButtonDown("Fire2"))
        {
           flag = false;
        }
        if (Input.GetKeyDown(SpawnKey))
        {
            flag = true;
        }
        if (flag)
        {
            GameObject ball = Instantiate(projectile, transform.position, transform.rotation);
        }
            
        return "";
    }
}
