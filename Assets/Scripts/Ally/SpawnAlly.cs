using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAlly : MonoBehaviour
{
    public GameObject Ally;
    public KeyCode SpawnKey = KeyCode.R;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(SpawnKey))
        {
            GameObject ally = Instantiate(Ally, transform.position, transform.rotation);
        }

    }
}
