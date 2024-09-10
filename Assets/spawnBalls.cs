using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnBalls : MonoBehaviour
{
    public GameObject projectile;
    public KeyCode SpawnKey = KeyCode.M;
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
        if (Input.GetKey(SpawnKey))
        {
            GameObject ball = Instantiate(projectile, transform.position, transform.rotation);
        }        
        return "";
    }
}
