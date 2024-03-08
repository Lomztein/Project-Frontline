using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerUnitSelection : MonoBehaviour, IHasTooltip
{
    public WorldPointer PlayerWorldPointer;
    public GameObject CurrentSelection;
    public LayerMask TargetLayerMask;
    public float SelectionRange;
    private Unit _currentSelectionUnit;

    public SelectionBoxVisual SelectionBoxHoverVisual;
    public SelectionBoxVisual SelectionBoxVisual;

    public UnityEvent<Unit> OnUnitSelected;
    public UnityEvent<Unit> OnUnitDeselected;

    public Tooltip PlayerTooltip;

    private void Update()
    {
        if (PlayerWorldPointer.PrimaryState || PlayerWorldPointer.SecondaryState)
        {
            ClearSelection();
        }

        Vector3 position = PlayerWorldPointer.WorldPosition;
        Collider[] colliders = Physics.OverlapSphere(position, SelectionRange, TargetLayerMask);
        float closestDist = float.MaxValue;
        Collider closest = null;
        foreach (var collider in colliders)
        {
            float dist = Vector3.SqrMagnitude(collider.transform.position - position);
            if (dist < closestDist)
            {
                closest = collider;
                closestDist = dist;
            }
        }
        if (closest != null)
        {
            SelectionBoxHoverVisual.gameObject.SetActive(true);
            SelectionBoxHoverVisual.Assign(closest.gameObject);

            if (PlayerWorldPointer.PrimaryState)
            {
                ClearSelection();
                SelectUnit(closest.gameObject);
            }
        }
        else
        {
            SelectionBoxHoverVisual.gameObject.SetActive(false);
        }
    }

    private void ClearSelection()
    {
        if (_currentSelectionUnit != null)
        {
            _currentSelectionUnit.Health.OnDeath -= Health_OnDeath;
            OnUnitDeselected?.Invoke(_currentSelectionUnit);
        }
        SelectionBoxVisual.gameObject.SetActive(false);
        CurrentSelection = null;
        _currentSelectionUnit = null;

        PlayerTooltip.ResetForcedTooltip();
    }

    private void SelectUnit(GameObject gameObject)
    {
        SelectionBoxVisual.gameObject.SetActive(true);
        CurrentSelection = gameObject;
        _currentSelectionUnit = gameObject.transform.root.GetComponentInChildren<Unit>();
        SelectionBoxVisual.Assign(gameObject);
        OnUnitSelected?.Invoke(_currentSelectionUnit);
        _currentSelectionUnit.Health.OnDeath += Health_OnDeath;

        PlayerTooltip.ResetForcedTooltip();
    }

    private void Health_OnDeath(Health obj)
    {
        obj.OnDeath -= Health_OnDeath;
        ClearSelection();
    }

    public GameObject InstantiateTooltip()
        => UnitTooltip.Create(_currentSelectionUnit, _currentSelectionUnit.InitialCommander); 
}
