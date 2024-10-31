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

    /// <summary>
    /// Moves the slider and changes the color depending on value
    /// </summary>
    /// <param name="currentValue">How much health the game object currently has</param>
    /// <param name="maxValue">How much health the game object starts with</param>
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
}
