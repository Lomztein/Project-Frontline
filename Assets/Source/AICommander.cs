using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AICommander : Commander
{
    public float TargetAvarageAPM = 20;

    public UnitWeightTableBase UnitWeightTable;

    private IUnitSelector _unitSelector;
    private IPositionSeletor _positionSelector;

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
        if (Random.Range(0, randLimit) == 0)
        {
            PerformAction();
        }
    }

    private void PerformAction()
    {
        GameObject unit = _unitSelector.SelectUnit(UnitSource.GetAvailableUnitPrefabs().Where(x => x.GetComponent<Unit>().Cost <= Credits));

        if (unit)
        {
            Vector3 position = _positionSelector.SelectPosition(new Vector3[] { Fortress.position }, GetUnitPlacementCheckSize(unit));
            TryPurchaseAndPlaceUnit(unit, position, transform.rotation);
        }
    }
}
