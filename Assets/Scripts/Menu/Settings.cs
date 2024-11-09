using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Settings : MonoBehaviour
{
    public int vsyncOptions = 0;
    public int frameRate = 0;
    public int oldVsyncOptions = 0;
    public Slider slider;
    public TextMeshProUGUI fpsNumber;

    void Start()
    {
        ChangeFrameRate();
        ChangeVsync();
    }

    public void ChangeFrameRate()
    {
        frameRate = (int)slider.value;

        if (slider.value == 9)
        {
            fpsNumber.text = "Unlimited";
            Application.targetFrameRate = 0;
        }
        else
        {
            fpsNumber.text = frameRate.ToString();
            Application.targetFrameRate = frameRate;
        }
    }

    //Doesn't seem to change anything
    void ChangeVsync()
    {
        if (vsyncOptions != oldVsyncOptions)
        {
            QualitySettings.vSyncCount = vsyncOptions;
            oldVsyncOptions = vsyncOptions;
        }
    }
}
