using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WeaponEditorBase<T> : Editor where T : class
{
    private bool _showAllDamage;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        IWeapon weapon = target as IWeapon;
        EditorGUILayout.LabelField("Raw DPS: " + weapon.GetDPSOrOverride());
        _showAllDamage = EditorGUILayout.BeginFoldoutHeaderGroup(_showAllDamage, "Damage / DPS against armor");
        if (_showAllDamage)
        {
            var values = Resources.LoadAll<DamageModifier>("DamageModifiers/Health");
            foreach (var value in values)
            {
                if (!value.Abstract)
                {
                    float factor = DamageModifier.Combine(value, weapon.Modifier);
                    EditorGUILayout.LabelField("Damage / DPS against " + value.ToString() + ": " + weapon.Damage * factor + " / " + weapon.GetDPSOrOverride() * factor);
                }
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : WeaponEditorBase<Weapon>
{
}

[CustomEditor(typeof(ChargingWeapon))]
public class ChargingWeaponEditor : WeaponEditorBase<ChargingWeapon>
{
}

[CustomEditor(typeof(WeaponGroup))]
public class WeaponGroupEditor : WeaponEditorBase<WeaponGroup>
{
}

[CustomEditor(typeof(UnitFactoryWeapon))]
public class UnitFactoryWeaponEditor : WeaponEditorBase<UnitFactoryWeapon>
{
}

// This is dumb and I love it.