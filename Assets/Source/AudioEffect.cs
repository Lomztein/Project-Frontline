using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioEffect : Effect
{
    // 0-256 is too.. open for me? I want some standardization here.
    // Higher priorities should be reserved for weapons that fire less commonly such as cannons, with lower priorities being machine guns.
    // Max should be ONLY for things that the player absolutely needs to hear.
    public enum Priority { Min = 256, VeryLow = 224, Low = 192, LowMedium = 160, Medium = 128, MediumHigh = 96, High = 64, VeryHigh = 32, Max = 256 }
    public Priority SoundPriority = Priority.Medium;

    public AudioSource Source;

    public override bool IsPlaying => Source.isPlaying;

    protected virtual void Start()
    {
        Source.priority = (int)SoundPriority;
    }

    public override void Stop()
    {
        Source.Stop();
    }
}
