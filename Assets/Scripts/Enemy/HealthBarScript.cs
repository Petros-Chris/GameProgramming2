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
    private GameObject whatCamItOn;

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
        if (whatCamItOn == null)
        {
            whatCamItOn = GameObject.Find("Death Cam");
            cam = whatCamItOn.GetComponent<Camera>();
            Debug.Log("Camera changed :>");
        }
        transform.rotation = cam.transform.rotation;
        transform.position = target.position + offSet;
    }
}
