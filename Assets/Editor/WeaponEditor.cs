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
        EditorGUILayout.LabelField("Raw DPS: " + weapon.GetDPS());
        _showAllDamage = EditorGUILayout.BeginFoldoutHeaderGroup(_showAllDamage, "Damage / DPS against armor");
        if (_showAllDamage)
        {
            var values = System.Enum.GetValues(typeof(DamageMatrix.Armor));
            foreach (var value in values)
            {
                float factor = DamageMatrix.GetDamageFactor(weapon.DamageType, (DamageMatrix.Armor)value);
                EditorGUILayout.LabelField("Damage / DPS against " + value.ToString() + ": " + weapon.Damage * factor + " / " + weapon.GetDPS() * factor);
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