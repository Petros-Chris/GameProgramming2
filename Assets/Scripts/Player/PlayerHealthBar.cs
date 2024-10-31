using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealthBar : MonoBehaviour
{
    public Slider slider;
    public Camera cam;
    public Transform orientation;
    public Transform target;
    public Vector3 offSet;

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;

        if (slider.value > 0.3f && slider.value < 0.5f)
        {
            slider.fillRect.GetComponent<Image>().color = Color.yellow;
        }
        else if (slider.value < 0.3f)
        {
            slider.fillRect.GetComponent<Image>().color = Color.red;
        }
    }

    private void Update()
    {
        /*
        Vector3 camUp = new Vector3(0, -cam.transform.up.x, 0);

        Vector3 healthBarPosition = orientation.position + (orientation.forward);
        healthBarPosition += camUp;
        healthBarPosition += -orientation.right / 2;
        transform.rotation = orientation.rotation;
        transform.position = healthBarPosition;
        */
    }
}
