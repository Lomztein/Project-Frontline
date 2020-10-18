using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEffect : Effect
{
    public TrailRenderer Trail;

    public override bool IsPlaying => Time.time > _stopTime + Trail.time;
    private float _stopTime;

    public override void Play()
    {
        base.Play();
        Trail.emitting = true;
    }

    public override void Stop()
    {
        Trail.emitting = false;
        _stopTime = Time.time;
    }
}
