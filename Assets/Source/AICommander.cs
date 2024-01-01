using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class AICommander : Commander
{
    public float TargetAvarageAPM = 20;
    public float ActionCooldown = 1f;
    public float QuickPlaceTime = 0.25f;
    private float _actionCooldownTime;
    public Vector2 SaveTimeMinMax;
    public AnimationCurve SaveTimeBias;

    private IUnitSelector _unitSelector;
    private IPositionSeletor _positionSelector;
    private int _highestUnitCost;

    public Unit SaveTarget;
    public int MaxPurchaseAtOnce = 8;

    protected override void Awake()
    {
        base.Awake();
        _unitSelector = GetComponent<IUnitSelector>();
        _positionSelector = GetComponent<IPositionSeletor>();
    }

    protected override void Start()
    {
        base.Start();
        _highestUnitCost = UnitSource.GetAvailableUnitPrefabs(Faction).Max(x => x.GetComponent<Unit>().BaseCost);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!Eliminated && _actionCooldownTime < 0f)
        {
            int randLimit = Mathf.RoundToInt((60 / Time.fixedDeltaTime) / TargetAvarageAPM);
            if (SaveTarget == null && Random.Range(0, randLimit) == 0)
            {
                PerformAction();
            }
            if (SaveTarget != null && CanAfford(SaveTarget, Credits))
            {
                Vector3? position = _positionSelector.SelectPosition(this, SaveTarget.gameObject, GetUnitPlacementOverlapShape(SaveTarget.gameObject));
                if (position.HasValue)
                {
                    TryPurchaseAndPlaceUnit(SaveTarget.gameObject, position.Value, transform.rotation);
                }
                SaveTarget = null;
            }
        }
        _actionCooldownTime -= Time.fixedDeltaTime;
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
        GameObject unit = _unitSelector.SelectUnit(UnitSource.GetAvailableUnitPrefabs(Faction).Where(x => CanAfford(x, (int)maxCost) && CanPurchase(x)));

        if (unit)
        {
            if (CanAfford(unit, Credits))
            {
                int affordableCount = Mathf.FloorToInt(Credits / (float)GetCost(unit));
                int toPlace = Mathf.Min(Random.Range(1, affordableCount + 1), MaxPurchaseAtOnce);
                StartCoroutine(PurchaseMultiple(unit, toPlace));
            }
            else
            {
                SaveTarget = unit.GetComponent<Unit>();
            }
            _actionCooldownTime = ActionCooldown;
        }
    }

    private IEnumerator PurchaseMultiple(GameObject unit, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3? position = _positionSelector.SelectPosition(this, unit, GetUnitPlacementOverlapShape(unit));
            if (position.HasValue)
            {
                TryPurchaseAndPlaceUnit(unit, position.Value, transform.rotation);
                yield return new WaitForSeconds(QuickPlaceTime);
            }
        }
    }

    private bool CanAfford(GameObject prefab, int credits)
    {
        return CanAfford(prefab.GetComponent<Unit>(), credits);
    }

    private bool CanAfford(Unit unit, int credits)
    {
        return unit.GetCost(this) < credits;
    }
}
