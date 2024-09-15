using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMovementScript : MonoBehaviour
{
    public Camera cam;
    public Transform orientation;
    public float smoothSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 canvasPosition = cam.transform.position; // player pos
        //canvasPosition += orientation.forward; //Moves it by 1 from the [1],0,0 pos
        //canvasPosition += orientation.up; // Moves it by 1 up from the 0,[1],0 pos
        canvasPosition += cam.transform.forward.normalized * 1.5f;
        canvasPosition += -cam.transform.right.normalized / 1.5f;
        canvasPosition += cam.transform.up.normalized /2;
        //transform.rotation = orientation.rotation; //Rotates it along the y axis for left and right movement
        transform.position = Vector3.Lerp(transform.position, canvasPosition, smoothSpeed * Time.deltaTime);

        Vector3 direction = (transform.position - cam.transform.position).normalized;
        direction.y = 0; // Keep the health bar level by nullifying any tilt on the y-axis
        Quaternion lookRotation = Quaternion.LookRotation(-direction); // Reverse direction to face the camera
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation , smoothSpeed * Time.deltaTime);
    }
}
