using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public enum DayNightBehaviour { AlwaysDay, AlwaysNight, Cycle }
    public DayNightBehaviour Behaviour;

    public float CycleLengthSeconds;
    public Vector2 PrimaryLightIntensity;
    public float CycleProgress;
    public float DayToNightFactor;
    public AnimationCurve SunRotationCurve;
    public AnimationCurve SunIntensityCurve;

    public Transform SunTransform;
    public Light SunLight;
    public Vector3 BaseRotation;

    public float AlwaysDayProgress = 0.5f;
    public float AlwaysNightProgress = 0f;

    public Text ClockText;

    void Update()
    {
        if (Behaviour == DayNightBehaviour.Cycle)
        {
            CycleProgress += Time.deltaTime / CycleLengthSeconds;
            CycleProgress %= 1f;
        }else if (Behaviour == DayNightBehaviour.AlwaysDay) {
            CycleProgress = AlwaysDayProgress;
        }
        else
        {
            CycleProgress = AlwaysNightProgress;
        }

        float sunProgress = SunRotationCurve.Evaluate(CycleProgress);
        float sunIntensity = SunIntensityCurve.Evaluate(CycleProgress);

        SunTransform.eulerAngles = new Vector3(BaseRotation.x + sunProgress * 360f, BaseRotation.y, BaseRotation.z);
        SunLight.intensity = sunIntensity;

        ClockText.text = GetClock(CycleProgress);
    }

    public string GetClock (float progress)
    {
        float hour = progress * 24f % 24f;
        float remainder = hour % 1f;
        float minute = remainder * 60f;
        remainder = minute % 1f;
        float second = remainder * 60f;
        return $"{Mathf.Floor(hour):00}:{Mathf.Floor(minute):00}:{Mathf.Floor(second):00}";
    }
}
