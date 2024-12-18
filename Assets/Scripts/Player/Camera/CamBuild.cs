using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamBuild : MonoBehaviour
{
    float yRotation;
    Vector3 moveDirection;
    public float sensX;
    public float sensY;
    public float verticalSpeed = 5.0f;
    public float speed = 10.0f;
    public float sprintSpeedMultiplier = 2.0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Locks camera in out person
        if (GameMenu.Instance.isPaused)
        {
            return;
        }

        CameraRotation();
        CameraMovement();

        ComponentManager.Instance.ToggleBuildPlayerMode();
    }

    private void CameraRotation()
    {
        // Locks mouse and allows rotation when right click is held
        if (Input.GetMouseButton(1))
        {
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            float mouseXAxis = Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * sensX;
            yRotation += mouseXAxis;
            transform.localRotation = Quaternion.Euler(65, yRotation, 0);
        }
        // Releases Mouse when right click is not held
        else if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void CameraMovement()
    {
        float keyboardXAxis = Input.GetAxisRaw("Horizontal");
        float keyboardYAxis = Input.GetAxisRaw("Vertical");

        Vector3 camForward = transform.forward;
        Vector3 camRight = transform.right;

        camForward.y = 0;
        camForward.Normalize();
        camRight.y = 0;
        camRight.Normalize();

        moveDirection = camRight * keyboardXAxis + camForward * keyboardYAxis;

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
