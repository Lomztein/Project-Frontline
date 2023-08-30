using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingWeaponAudioCue : MonoBehaviour
{
    public ChargingWeapon Weapon;
    public AudioSource AudioSource;
    public float BaseVolume;
    public AnimationCurve VolumeOverCharge = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve PitchOverCharge = AnimationCurve.Constant(0, 1, 1);

    public Vector2 AudioTriggerThresholdMinMax;

    // Update is called once per frame
    void FixedUpdate()
    {
        float factor = Weapon.CurrentChargeTime / Weapon.MaxChargeTime;
        bool trigger = factor > AudioTriggerThresholdMinMax.x && factor < AudioTriggerThresholdMinMax.y;
        if (trigger && !AudioSource.isPlaying)
        {
            AudioSource.Play();
        }
        if (AudioSource.isPlaying && !trigger)
        {
            AudioSource.Stop();
        }
        AudioSource.volume = BaseVolume * VolumeOverCharge.Evaluate(factor);
        AudioSource.pitch = PitchOverCharge.Evaluate(factor);
    }
}
