using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffect : Effect
{
    public ParticleSystem System;

    public override bool IsPlaying => System.IsAlive(true);

    public override void Play()
    {
        base.Play();
        System.Play();
    }

    public override void Stop()
    {
        System.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
