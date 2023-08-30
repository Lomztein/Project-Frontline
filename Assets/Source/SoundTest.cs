using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    public AudioSource Source;
    public AudioClip[] Clips;

    private void Start()
    {
        InvokeRepeating(nameof(Play), 1f, 1f);
    }

    private void Play()
    {
        AudioClip clip = Clips[Random.Range(0, Clips.Length)];
        Source.pitch = Random.Range(0.9f, 1.1f);
        Source.PlayOneShot(clip);
    }
}
