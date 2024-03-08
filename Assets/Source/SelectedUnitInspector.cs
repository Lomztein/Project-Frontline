using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectedUnitInspector : MonoBehaviour, IHasTooltip
{
    public CameraSelector CameraSelector;
    public Unit CurrentUnit;
    public Tooltip PlayerTooltip;
    public ComponentInspector[] Inspectors;
    private Dictionary<Component, GameObject> _inspectorUIs = new Dictionary<Component, GameObject>();

    public GameObject InstantiateTooltip()
    {
        GameObject baseTooltip = UnitTooltip.Create(CurrentUnit, CurrentUnit.InitialCommander);
        Component[] components = CurrentUnit.GetComponentsInChildren<Component>();

        foreach (var component in components)
        {
            foreach (var inspector in Inspectors)
            {
                if (inspector.CanInspect(component))
                {
                    GameObject tooltipAddition = inspector.InstantiateInspectUI(component);
                    _inspectorUIs.Add(component, tooltipAddition);
                    tooltipAddition.transform.SetParent(baseTooltip.transform);
                }
            }
        }

        return baseTooltip;
    }

    private void Update()
    {
        if (CurrentUnit)
        {
            PlayerTooltip.ForceTooltip(this, CameraSelector.CurrentCamera.WorldToScreenPoint(CurrentUnit.transform.position));
            foreach (var pair in _inspectorUIs)
            {
                ComponentInspector inspector = Inspectors.FirstOrDefault(x => x.CanInspect(pair.Key));
                if (inspector != null)
                {
                    inspector.UpdateInspectorUI(pair.Key, pair.Value); 
                }
            }
        }
        else if (PlayerTooltip.IsForced)
        {
            PlayerTooltip.ResetForcedTooltip();
        }
    }

    public void OnUnitSelected(Unit unit)
    {
        _inspectorUIs.Clear();
        CurrentUnit = unit;
    }

    public void OnUnitDeselected(Unit unit)
    {
        _inspectorUIs.Clear();
        CurrentUnit = null;
    }
}
