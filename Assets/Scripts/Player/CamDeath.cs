using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CamDeath : MonoBehaviour
{    
    float xRotation;
    float yRotation;
    Vector3 moveDirection;

    public float sensX;
    public float sensY;
    public float verticalSpeed = 5.0f;
    public float speed = 10.0f;
    public float sprintSpeedMultiplier = 2.0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        CameraRotation();
        CameraMovement();
    }

    private void CameraRotation()
    {
        float mouseXAxis = Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * sensX;
        float mouseYAxis = Input.GetAxisRaw("Mouse Y") * Time.fixedDeltaTime * sensY;
        
        yRotation += mouseXAxis;
        xRotation -= mouseYAxis;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

    private void CameraMovement()
    {
        float keyboardXAxis = Input.GetAxisRaw("Horizontal");
        float keyboardYAxis = Input.GetAxisRaw("Vertical");

        moveDirection = transform.right * keyboardXAxis + transform.forward * keyboardYAxis;

        if (Input.GetKey(KeyCode.Space))
        {
            moveDirection.y = verticalSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            moveDirection.y = -verticalSpeed;
        }
        
        Vector3 moveSpeed = speed * Time.deltaTime * moveDirection;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed *= sprintSpeedMultiplier;
        }

        transform.position += moveSpeed;
    }
}