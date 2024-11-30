using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAlly : MonoBehaviour
{
    public GameObject Ally;
    private KeyCode SpawnKey = KeyCode.P;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(SpawnKey))
        {
            Instantiate(Ally, transform.position, transform.rotation);
        }

    }
}
