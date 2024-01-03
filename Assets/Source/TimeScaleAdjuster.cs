using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScaleAdjuster : MonoBehaviour
{
    public static float TimeScale
    {
        get => Time.timeScale;
        set => Time.timeScale = value;
    }

    private const int BASE_FIXED_TIME_STEP_FREQUENCY = 50;

    public TMPro.TMP_Dropdown ModeDropdown;
    public Text PercentText;
    public Slider Slider;

    private FloatMovingWindow FramerateTracker = new FloatMovingWindow(10);
    public float TargetMinFramerate;
    public float TargetMaxFrameTime => 1f / TargetMinFramerate;
    public float AutoAdjustP;

    void Update()
    {
        FramerateTracker.Register(Time.deltaTime);

        if (ModeDropdown.value == 0)
        {
            TimeScale = 1f;
        }
        if (ModeDropdown.value == 1)
        {
            TimeScale = Slider.value;
        }
        if (ModeDropdown.value == 2)
        {
            AutoAdjust(FramerateTracker.Average());
        }


        Slider.value = TimeScale;
        PercentText.text = TimeScale.ToString("P0");
    }

    private void AutoAdjust(float dt)
    {
        float error = dt - TargetMaxFrameTime;
        TimeScale -= error * AutoAdjustP;
        TimeScale = Mathf.Clamp(TimeScale, 0.1f, 1f);
    }
}
