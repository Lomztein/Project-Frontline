using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlash : Effect
{
    public Light Light;
    public float MaxIntensity;
    public AnimationCurve IntensityCurve;
    public float FalloffTime;
    public bool ScaleIntensity;

    private bool _isPlaying;
    private float _time;
    private float _scale;

    public override bool IsPlaying => _isPlaying;

    private void Awake()
    {
        if (ScaleIntensity)
        {
            _scale = transform.lossyScale.magnitude;
        }
        else
        {
            _scale = 1f;
        }
    }

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
        Light.intensity = MaxIntensity * IntensityCurve.Evaluate(time) * _scale;
    }

    public override void Stop()
    {
        UpdateLight(_time);
        _time = 1f;
        _isPlaying = false;
        Light.enabled = false;
    }
}
