using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class UnitValueMassEditor : EditorWindow
{
    public string UnitPathRoot = "Assets/Resources/Units";

    public float GlobalValueMultiplier = 0.1f;
    public float BaseProductionTime = 40f;
    public float FallbackProductionTime = 120f;
    public float[] UnitValueMultByTier = new float[] { 0.25f, 0.5f, 1f, 5f };
    public float[] UnitValueMultByType = new float[] { 0.5f, 1f, 1f, 2f, 2f, 1f };

    private bool _tierFoldout;
    private bool _typeFoldout;

    [MenuItem("Project Frontline/Unit Value Mass Editor")]
    public static void ShowWindow()
    {
        GetWindow<UnitValueMassEditor>();
    }

    public void OnGUI()
    {
        var tiers = Enum.GetValues(typeof(UnitInfo.Tier));
        var types = Enum.GetValues(typeof(UnitInfo.Type));

        if (UnitValueMultByTier == null || UnitValueMultByTier.Length != tiers.Length)
            UnitValueMultByTier = new float[tiers.Length];

        if (UnitValueMultByType == null || UnitValueMultByType.Length != types.Length)
            UnitValueMultByType = new float[types.Length];

        UnitPathRoot = EditorGUILayout.TextField("Unit path root", UnitPathRoot);
        GlobalValueMultiplier = EditorGUILayout.FloatField("Global divisor", GlobalValueMultiplier);
        BaseProductionTime = EditorGUILayout.FloatField("Base production time", BaseProductionTime);
        FallbackProductionTime = EditorGUILayout.FloatField("Fallback production time", FallbackProductionTime);

        _tierFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_tierFoldout, "Value by tier");
        if (_tierFoldout)
        {
            for (int i = 0; i < tiers.Length; i++)
            {
                UnitValueMultByTier[i] = EditorGUILayout.FloatField(tiers.GetValue(i).ToString(), UnitValueMultByTier[i]);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        _typeFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_typeFoldout, "Value by type");
        if (_typeFoldout)
        {
            for (int i = 0; i < types.Length; i++)
            {
                UnitValueMultByType[i] = EditorGUILayout.FloatField(types.GetValue(i).ToString(), UnitValueMultByType[i]);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (GUILayout.Button("Update values!"))
        {
            UpdateValues();
        }
    }

    private void UpdateValues()
    {
        string[] files = Directory.GetFiles(UnitPathRoot, "*.prefab", SearchOption.AllDirectories);
        Debug.Log(files.Length);
        foreach (var file in files)
        {
            var unitGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(file);
            if (unitGameObject.TryGetComponent(out Unit unit))
            {
                unit.Info.Value = ComputeValue(unit);
            }
        }

        AssetDatabase.SaveAssets();
    }

    private int ComputeValue(Unit unit)
    {
        float productionTime = FallbackProductionTime;
        if (unit.TryGetComponent(out ProductionInfo info))
        {
            productionTime = info.ProductionTime;
        }
        float productionFactor = productionTime / BaseProductionTime;

        float result = unit.Info.Cost * GlobalValueMultiplier;
        result *= UnitValueMultByTier[(int)unit.Info.UnitTier];
        result *= UnitValueMultByType[(int)unit.Info.UnitType];
        return Mathf.RoundToInt(result * productionFactor);
    }
}
