using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Unit))]
public class UnitEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label(new GUIContent("Unit power: " + UnitPowerUtil.ComputeUnitPower(target as Unit), "Arbitrarily calculated value. Useful for comparing units of similar role, but should not be relied upon."));
        GUILayout.Label("Cost / power: " + UnitPowerUtil.ComputeUnitPower(target as Unit) / (target as Unit).BaseCost);
    }
}
