using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    Slider slider;
    Camera cam;
    Transform target;
    public Vector3 offSet;
    GameObject whatCamItOn;
    bool camChanged = false;

    private void Awake()
    {
        whatCamItOn = GameObject.Find("Player Camera");
        if (whatCamItOn != null)
        {
            cam = whatCamItOn.GetComponent<Camera>();
        }
        slider = gameObject.GetComponent<Slider>();
        target = gameObject.transform.parent;
    }

    /// <summary>
    /// Moves the slider and changes the color depending on value
    /// </summary>
    /// <param name="currentValue">How much health the game object currently has</param>
    /// <param name="maxValue">How much health the game object starts with</param>
    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;

        if (slider.value <= 0.5f && slider.value > 0.3f)
        {
            slider.fillRect.GetComponent<Image>().color = Color.yellow;
        }
        else if (slider.value <= 0.3)
        {
            slider.fillRect.GetComponent<Image>().color = Color.red;
        }
    }

    private void Update()
    {
        //To stop it constantly reassigning the component
        if (camChanged == false)
        {
            //Changes the camera the health bar is assigned to if it ends up getting destroyed
            if (GameObject.Find("Player Camera") == null)
            {
                if (GameObject.Find("Death Cam") != null)
                {
                    whatCamItOn = GameObject.Find("Death Cam");
                    cam = whatCamItOn.GetComponent<Camera>();
                    camChanged = true;
                }
                else if (GameObject.Find("BuildCamera") != null)
                {
                    whatCamItOn = GameObject.Find("BuildCamera");
                    cam = whatCamItOn.GetComponent<Camera>();
                    camChanged = true;
                }
            }
        }
        if (camChanged == true)
        {
            //Changes the camera the health bar is assigned to if it ends up coming back
            if (GameObject.Find("Player Camera") != null)
            {
                whatCamItOn = GameObject.Find("Player Camera");
                cam = whatCamItOn.GetComponent<Camera>();
                camChanged = false;
                Debug.Log("Camera changed :>");
            }
        }
        transform.rotation = cam.transform.rotation;
        transform.position = target.position + offSet;
    }
}
