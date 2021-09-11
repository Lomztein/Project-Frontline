using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WeaponEditorBase<T> : Editor where T : class
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        IWeapon weapon = target as IWeapon;
        EditorGUILayout.LabelField("DPS: " + weapon.Damage * weapon.Firerate);
    }
}

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : WeaponEditorBase<Weapon>
{
}

[CustomEditor(typeof(WeaponGroup))]
public class WeaponGroupEditor : WeaponEditorBase<WeaponGroup>
{
}

// This is dumb and I love it.