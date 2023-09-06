using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomWeaponAimBehaviour : WeaponAimBehaviour
{
    public float ZoomFactor;
    public bool ZoomWeaponCam;
    public AnimationCurve ZoomCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    public override void SetAim(IWeapon weapon, float factor)
    {
        float val = ZoomCurve.Evaluate(factor);
        float defaultVertical = Camera.HorizontalToVerticalFieldOfView(Posessor.FirstPersonBaseFoV, Camera.main.aspect);
        Posessor.Instance.MainPosessedCamera.fieldOfView = Mathf.Lerp(defaultVertical, defaultVertical / ZoomFactor, val);
        if (ZoomWeaponCam)
        {
            float defaultWeaponsVertical = Camera.HorizontalToVerticalFieldOfView(Posessor.ThirdPersonBaseFoV, Camera.main.aspect);
            Posessor.Instance.WeaponPosessedCamera.fieldOfView = Mathf.Lerp(defaultWeaponsVertical, defaultWeaponsVertical / ZoomFactor, val);
        }
        Posessor.CameraSensitivity = Posessor.CameraBaseSensivity / ZoomFactor;
    }
}
