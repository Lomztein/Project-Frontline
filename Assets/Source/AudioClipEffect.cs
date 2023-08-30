using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipEffect : AudioEffect
{
    public AudioClip Clip;
    public AudioClip[] RandomClips;
    public float VolumeScale = 1f;
    public float PitchVariance;
    public Object LimiterKey;

    public override bool IsPlaying => Source.isPlaying;

    protected override void Start()
    {
        base.Start();
    }

    public override void Play()
    {
        base.Play();
        AudioClip clip = Clip != null ? Clip : RandomClips[Random.Range(0, RandomClips.Length - 1)];
        Object key = LimiterKey == null ? clip : LimiterKey;

        if (AudioManager.RequestPlay(clip, key, Source.transform.position))
        {
            Source.pitch = Random.Range(1 - PitchVariance, 1 + PitchVariance);
            Source.PlayOneShot(clip, VolumeScale);
        }
    }
}
