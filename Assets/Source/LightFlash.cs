using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlash : Effect
{
    public Light Light;
    public float MaxIntensity;
    public float FalloffTime;

    private bool _isPlaying;
    private float _time;

    public enum Falloff { Liniar, Exponential, Power }
    public Falloff FalloffType;

    private Dictionary<Falloff, Func<float, float, float>> _falloffMap = new Dictionary<Falloff, Func<float, float, float>>()
    {
        { Falloff.Liniar, (t, max) => (1-t) * max }
    };

    public override bool IsPlaying => _isPlaying;

    public override void Play()
    {
        base.Play();
        _isPlaying = true;
        Light.enabled = true;
        _time = 0;
    }

    private void FixedUpdate()
    {
        if (_isPlaying)
        {
            _time += Time.fixedDeltaTime / FalloffTime;
            UpdateLight(Mathf.Clamp01(_time));

            if (_time >= 1)
            {
                Stop();
            }
        }
    }

    private void UpdateLight (float time)
    {
        Light.intensity = _falloffMap[FalloffType](time, MaxIntensity);
    }

    public override void Stop()
    {
        UpdateLight(_time);
        _time = 1f;
        _isPlaying = false;
        Light.enabled = false;
    }
}
