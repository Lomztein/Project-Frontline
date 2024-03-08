using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EquipmentInfoList))]
public class UnitEquipmentInfoEditor : Editor
{
    private static Dictionary<Type, string> _defaultNames = new Dictionary<Type, string>()
    {
        { typeof(Health), "Health" },
        { typeof(IControllable), "Engine" },
        { typeof(ITurret), "Turret" },
        { typeof(IWeapon), "Weapon" }
    };

    private static Dictionary<Type, string> _defaultDescs = new Dictionary<Type, string>()
    {
        { typeof(Health), "The hull of the unit. Destroying this kills the unit." },
        { typeof(IControllable), "The engine of the unit. This allows the unit to move." },
        { typeof(ITurret), "This allows the unit to aim towards its target." },
        { typeof(IWeapon), "Allows the unit to attack its targets." }
    };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate from components"))
        {
            GenerateFromEquipmentComponents();
        }
    }

    private void GenerateFromEquipmentComponents()
    {
        EquipmentInfoList targetList = target as EquipmentInfoList;

        targetList.ResetList();
        Component[] currentComponents = targetList.InfoList.Select(x => x.MainComponent).ToArray();
        List<Component> desiredComponents = new List<Component>();

        Unit unit = targetList.GetComponentInParent<Unit>();
        desiredComponents.AddRange(unit.GetComponentsInChildren<Health>());
        desiredComponents.Add(unit.GetComponentInChildren<IControllable>() as Component);

        desiredComponents.AddRange(unit.GetWeapons().Cast<Component>());
        desiredComponents.AddRange(unit.GetComponentsInChildren<ITurret>().Cast<Component>());

        foreach (var component in desiredComponents)
        {
            if (!currentComponents.Contains(component))
            {
                EquipmentInfo newInfo = targetList.gameObject.AddComponent<EquipmentInfo>();
                newInfo.MainComponent = component;
                newInfo.ParentOverride = component.transform;

                foreach (var kvp in _defaultNames)
                {
                    if (kvp.Key.IsAssignableFrom(component.GetType()))
                    {
                        newInfo.Name = kvp.Value;
                    }
                }

                foreach (var kvp in _defaultDescs)
                {
                    if (kvp.Key.IsAssignableFrom(component.GetType()))
                    {
                        newInfo.Description = kvp.Value;
                    }
                }
            }
        }

        targetList.ResetList();
    }
}
