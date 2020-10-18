using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEffect : Effect
{
    public TrailRenderer Trail;

    public override bool IsPlaying => Trail.emitting;

    public override void Play()
    {
        base.Play();
        Trail.emitting = true;
    }

    public override void Stop()
    {
        Trail.emitting = false;
    }
}
