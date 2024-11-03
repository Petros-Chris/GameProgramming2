using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public int vsyncOptions = 0;
    public int frameRate = 0;
    private int oldFrameRate = 0, oldVsyncOptions = 0;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ChangeFrameRate();
        ChangeVsync();
    }
    //? Interestingly, even at 1 fps, it consumes the same amount of resources as with no limit
    void ChangeFrameRate()
    {
        if (frameRate != oldFrameRate)
        {
            Application.targetFrameRate = frameRate;
            oldFrameRate = frameRate;
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
