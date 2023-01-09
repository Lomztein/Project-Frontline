using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipEffect : Effect
{
    public AudioClip Clip;
    public float VolumeScale = 1f;
    public AudioSource Source;

    public override bool IsPlaying => Source.isPlaying;

    public override void Play()
    {
        base.Play();
        Source.PlayOneShot(Clip, VolumeScale);
    }

    public override void Stop()
    {
        Source.Stop();
    }
}
