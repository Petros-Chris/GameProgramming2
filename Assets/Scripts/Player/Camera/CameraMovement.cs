using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public Transform orientation;
    public Transform mesh;
    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Locks camera in first person
        if (GameMenu.isPaused)
        {
            return;
        }

        float mouseXAxis = Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * sensX;
        float mouseYAxis = Input.GetAxisRaw("Mouse Y") * Time.fixedDeltaTime * sensY;

        yRotation += mouseXAxis;
        xRotation -= mouseYAxis;
        //Prevents player from breaking their neck
        xRotation = Mathf.Clamp(xRotation, -80f, 60f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        if (orientation != null)
        {
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
            // I think this is getting hit with gimbal lock :O
            mesh.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }
}
