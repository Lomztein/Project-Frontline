using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileBodyAudio : MonoBehaviour
{
    public AudioSource Source;
    public MobileBody Body;

    public Vector2 PitchMinMax = new Vector2(0, 1);
    public Vector2 VolumeMinMax = new Vector2(0, 1);
    
    public AnimationCurve PitchOverSpeed = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public AnimationCurve VolumeOverSpeed = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    // Update is called once per frame
    void FixedUpdate()
    {
        float speedFactor = Body.CurrentSpeed / Body.MaxSpeed;
        Source.volume = Mathf.Lerp(VolumeMinMax.x, VolumeMinMax.y, VolumeOverSpeed.Evaluate(speedFactor));
        Source.pitch = Mathf.Lerp(PitchMinMax.x, PitchMinMax.y, PitchOverSpeed.Evaluate(speedFactor));
    }
}
