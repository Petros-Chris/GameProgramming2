using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMovementScript : MonoBehaviour
{
    public Camera cam;
    public Transform orientation;
    public float smoothSpeed;
    public Vector3 screenOffset = new Vector3(0.2f, 0.8f, 1.5f);

    void Update()
    {
        Vector3 viewportPosition = new Vector3(screenOffset.x, screenOffset.y, screenOffset.z);
        Vector3 canvasPosition = cam.ViewportToWorldPoint(viewportPosition);

        /*
        Vector3 canvasPosition = cam.transform.position;
        canvasPosition += cam.transform.forward.normalized * 1.5f;
        canvasPosition += -cam.transform.right.normalized / 1.5f;
        canvasPosition += cam.transform.up.normalized / 2;
        */
        transform.position = Vector3.Lerp(transform.position, canvasPosition, smoothSpeed * Time.deltaTime);

        //This effy
        Vector3 direction = (transform.position - cam.transform.position).normalized;
        direction.y = 0; // Keep the health bar level by nullifying any tilt on the y-axis
        Quaternion lookRotation = Quaternion.LookRotation(-direction); // Reverse direction to face the camera

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation , smoothSpeed * Time.deltaTime);
    }
}
