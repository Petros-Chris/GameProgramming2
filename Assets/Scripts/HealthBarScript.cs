using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Slider slider;
    public Camera cam;
    public Transform target;
    public Vector3 offSet;

    private void Awake()
    {
        //slider = GetComponent<Slider>();
        cam = GameObject.Find("Player Camera").GetComponent<Camera>();
        //target = transform;
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;

        if (slider.value < 0.3f)
        {
            slider.fillRect.GetComponent<Image>().color = Color.red;
        }
    }

    private void Update()
    {
        transform.rotation = cam.transform.rotation;
        transform.position = target.position + offSet;
    }
}
