using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScaleAdjuster : MonoBehaviour
{
    private const int BASE_FIXED_TIME_STEP_FREQUENCY = 50;

    public Text PercentText;
    public Slider Slider;
    public Toggle Toggle;

    private bool _enabled = false;

    void Update()
    {
        if (Toggle.isOn)
        {
            Time.timeScale = Slider.value;
        }
        else
        {
            Time.timeScale = 1f;
        }
        Time.fixedDeltaTime = Mathf.Max(Time.timeScale / BASE_FIXED_TIME_STEP_FREQUENCY, 0.00001f);
        PercentText.text = Slider.value.ToString("P0");
    }
}
