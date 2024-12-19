using System;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;


public class Settings : MonoBehaviour
{
    public int vsyncOptions = 0;
    public int frameRate = 0;
    public Slider fpsSlider;
    public TextMeshProUGUI fpsNumber;
    public GameObject fpsDisplay;
    // Slider
    public Slider vsyncSlider;
    public TextMeshProUGUI vsyncNumber;
    public string soundPath;
    public bool eggsMode;

    bool toggleCount;

    void Start()
    {
        eggsMode = false;
        soundPath = "SoundFX";
        ChangeFrameRate();
        ChangeVsync();
        JsonHandler.Save();
        JsonHandler.DataToSave data = JsonHandler.ReadFile("settings");
        frameRate = data.frameRate;
        vsyncOptions = data.vsyncOption;
    }

    public void ChangeFrameRate()
    {
        frameRate = (int)fpsSlider.value;

        if (vsyncOptions != 0)
        {
            vsyncNumber.text = "Off";
            QualitySettings.vSyncCount = 0;
            vsyncSlider.value = 0;
        }

        if (fpsSlider.value == 9)
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
    public void ChangeVsync()
    {
        vsyncOptions = (int)vsyncSlider.value;

        if (frameRate != 0)
        {
            fpsNumber.text = "Unlimited";
            Application.targetFrameRate = 0;
            fpsSlider.value = 0;
        }

        switch (vsyncOptions)
        {
            case 0:
                QualitySettings.vSyncCount = 0;
                vsyncNumber.text = "Off";
                break;
            case 1:
                QualitySettings.vSyncCount = 1;
                vsyncNumber.text = "Refresh Rate";
                break;
            case 2:
                QualitySettings.vSyncCount = 2;
                vsyncNumber.text = "1/2 Refresh Rate";
                break;
            case 3:
                QualitySettings.vSyncCount = 3;
                vsyncNumber.text = "1/3 Refresh Rate";
                break;
            case 4:
                QualitySettings.vSyncCount = 4;
                vsyncNumber.text = "1/4 Refresh Rate";
                break;
        }
    }

    //TODO: Get it to see the isOn in the toggle itself
    public void DisplayFps(bool isOn)
    {
        toggleCount = !toggleCount;
        fpsDisplay.SetActive(toggleCount);
    }

    //TODO: Get it to see the isOn in the toggle itself
    public void ToggleScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;

        // if (!Screen.fullScreen)
        // {
        //     Screen.SetResolution(640, 480, false);
        // }
        // else
        // {
        //     Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
        // }
    }

    public void ChangeScreenResoultion()
    {
        Screen.SetResolution(640, 480, false);
    }

    public void ChangeEggsMode(){
        eggsMode = !eggsMode;
        if(eggsMode){
            soundPath = "EggsSoundFX";
        }else{
            soundPath = "SoundFX";
        }

    }
}
