using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AICommander : Commander
{
    public float TargetAvarageAPM = 20;
    public Vector2 SaveTimeMinMax;
    public AnimationCurve SaveTimeBias;

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
        if (!Eliminated)
        {
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
    }

    private float GetExpectedCreditsAfterSaveTime (float time)
    {
        return Credits + AverageIncomePerSecond * time;
    }

    private void PerformAction()
    {
        SaveTarget = null;
        float time = Mathf.Lerp(SaveTimeMinMax.x, SaveTimeMinMax.y, SaveTimeBias.Evaluate(Random.Range(0f, 1f)));
        float maxCost = GetExpectedCreditsAfterSaveTime(time);
        GameObject unit = _unitSelector.SelectUnit(UnitSource.GetAvailableUnitPrefabs(Faction).Where(x => CanAfford(x, (int)maxCost)));

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
