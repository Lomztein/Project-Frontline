using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(ArtillaryTurret))]
public class ArtillaryTurretEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ArtillaryTurret turret = target as ArtillaryTurret;
        EditorGUILayout.LabelField("Approximate range: " + turret.GetApproxProjectileRange(45f, 0f, turret.ProjectileSpeed, turret.ProjectileGravity));
    }
}
