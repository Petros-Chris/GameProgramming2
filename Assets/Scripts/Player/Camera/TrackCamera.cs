using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCamera : MonoBehaviour
{
    //Appartly this helps with preventing bugs with physics and the camera

    public Transform cameraPosition;

    void Update()
    {
        //moves the camera to the player position
        if(cameraPosition != null)
        {
            transform.position = cameraPosition.position;
        }
    }
}
