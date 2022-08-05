using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AICommander : Commander
{
    public float TargetAvarageAPM = 20;
    public float MaxSaveTime;

    private IUnitSelector _unitSelector;
    private IPositionSeletor _positionSelector;

    public Unit SaveTarget;

    protected override void Awake()
    {
        base.Awake();
        _unitSelector = GetComponent<IUnitSelector>();
        _positionSelector = GetComponent<IPositionSeletor>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        int randLimit = Mathf.RoundToInt((60 / Time.fixedDeltaTime) / TargetAvarageAPM);
        if (SaveTarget == null && Random.Range(0, randLimit) == 0)
        {
            PerformAction();
        }
        if (SaveTarget != null && CanAfford(SaveTarget, Credits))
        {
            Vector3? position = _positionSelector.SelectPosition(this, SaveTarget.gameObject, GetUnitPlacementCheckSize(SaveTarget.gameObject));
            if (position.HasValue)
            {
                TryPurchaseAndPlaceUnit(SaveTarget.gameObject, position.Value, transform.rotation);
            }
            SaveTarget = null;
        }
    }

    private float GetExpectedCreditsAfterMaxSaveTime ()
    {
        return Credits + AverageIncomePerSecond * MaxSaveTime;
    }

    private void PerformAction()
    {
        SaveTarget = null;
        float maxCost = GetExpectedCreditsAfterMaxSaveTime();
        GameObject unit = _unitSelector.SelectUnit(UnitSource.GetAvailableUnitPrefabs().Where(x => CanAfford(x, (int)maxCost)));

        if (unit)
        {
            if (CanAfford(unit, Credits))
            {
                Vector3? position = _positionSelector.SelectPosition(this, unit, GetUnitPlacementCheckSize(unit));
                if (position.HasValue)
                {
                    TryPurchaseAndPlaceUnit(unit, position.Value, transform.rotation);
                }
            }
            else
            {
                SaveTarget = unit.GetComponent<Unit>();
            }
        }
    }

    private bool CanAfford(GameObject prefab, int credits)
    {
        return CanAfford(prefab.GetComponent<Unit>(), credits);
    }

    private bool CanAfford(Unit unit, int credits)
    {
        return unit.Cost < credits;
    }
}
