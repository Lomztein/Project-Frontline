using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioControlUI : MonoBehaviour
{
    public AudioMixer Mixer;
    public string[] ParameterNames;
    public Slider[] Sliders;
    public Vector2 Range = new Vector2(-80f, 20f);

    private void Start()
    {
        for (int i = 0; i < Sliders.Length; i++) 
        {
            Sliders[i].value = Mathf.InverseLerp(Range.x, Range.y, 0f);
            RegisterEvent(Sliders[i], i);
        }
    }

    private void RegisterEvent(Slider slider, int index)
    {
        slider.onValueChanged.AddListener((x) =>
        {
            Mixer.SetFloat(ParameterNames[index], Mathf.Lerp(Range.x, Range.y, x));
        });
    }
}
