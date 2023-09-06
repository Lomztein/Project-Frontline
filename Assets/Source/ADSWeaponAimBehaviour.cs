using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADSWeaponAimBehaviour : ZoomWeaponAimBehaviour
{
    public Vector3 ADSLocalPosition;

    public override void SetAim(IWeapon weapon, float factor)
    {
        base.SetAim(weapon, factor);
        if (weapon is Component comp)
        {
            comp.transform.localPosition = Vector3.Lerp(Vector3.zero, ADSLocalPosition, factor);
        }
        Color color = PosessorUI.Instance.TargetingImage.color;
        color.a = 1f-factor;
        PosessorUI.Instance.TargetingImage.color = color;
    }
}
