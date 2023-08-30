using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingAudioClipEffect : AudioEffect
{
    public float StopThreshold;
    private float _lastTriggerTime;

    public override void Play()
    {
        base.Play();
        if (!Source.isPlaying && AudioManager.RequestPlay(Source.clip, Source.clip, transform.position))
        {
            Source.Play();
        }
        _lastTriggerTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (_lastTriggerTime + StopThreshold < Time.time) 
        {
            Stop();
        }
    }
}
