using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static DamageMatrix;

public class DamageMatrixConverter : EditorWindow
{
    public const string WEAPON_PATH = "Assets/Resources/DamageModifiers/Weapons";
    public const string HEALTH_PATH = "Assets/Resources/DamageModifiers/Health";

    [MenuItem("Project Frontline/Damage Matrix Converter")]
    public static void OpenWindow()
    {
        GetWindow<DamageMatrixConverter>();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Convert!"))
        {
            Convert();
        }
    }

    private void Convert()
    {
        /*var damageTypes = Enum.GetValues(typeof(DamageMatrix.Damage));
        var armorTypes = Enum.GetValues(typeof(DamageMatrix.Armor));

        List<CascadeDamageModifier> damageModifiers = new List<CascadeDamageModifier>();
        List<CascadeDamageModifier> armorModifiers = new List<CascadeDamageModifier>();

        foreach (var damage in damageTypes)
        {
            CascadeDamageModifier modifier = CreateInstance<CascadeDamageModifier>();
            modifier.Name = damage.ToString();
            AssetDatabase.CreateAsset(modifier, WEAPON_PATH + "/" + damage.ToString() + ".asset");
            damageModifiers.Add(modifier);
        }

        foreach (var armor in armorTypes)
        {
            CascadeDamageModifier modifier = CreateInstance<CascadeDamageModifier>();
            modifier.Name = armor.ToString();
            AssetDatabase.CreateAsset(modifier, HEALTH_PATH + "/" + armor.ToString() + ".asset");
            armorModifiers.Add(modifier);
        }

        // I know this is horrid shut up.
        for (int d = 0; d < damageModifiers.Count; d++)
        {
            DamageMatrix.Damage dam = (Damage)damageTypes.GetValue(d);
            var dlist = new List<Tuple<DamageModifier, float>>();

            for (int a = 0; a < armorModifiers.Count; a++)
            {
                DamageMatrix.Armor arm = (Armor)armorTypes.GetValue(a);
                float factor = DamageMatrix.GetDamageFactor(dam, arm);
                dlist.Add(new Tuple<DamageModifier, float>(armorModifiers[a], factor));
            }

            damageModifiers[d].Options = dlist.Select(x => new CascadeDamageModifier.Option()
            {
                Name = x.Item1.Name,
                AnyOf = new DamageModifier[] { x.Item1 },
                Value = x.Item2
            }).ToArray();
        }
        */
    }
}
