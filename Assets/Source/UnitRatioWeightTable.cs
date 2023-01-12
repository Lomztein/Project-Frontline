using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ratio Weight Table", menuName = "Unit Weight Tables/Ratio")]
public class UnitRatioWeightTable : UnitWeightTable
{
    public UnitFilter CountFilter;
    public bool CountUseFactoryUnit;
    public UnitFilter DesiredFilter;
    public bool DesiredUseFactoryUnit;
    public bool CountDesiredFromEnemies;

    public float DesiredRatio;
    public float NonDesiredWeight;

    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        int amountCounted = CountDesiredFromEnemies ?
            Team.GetOtherTeams(Commander.TeamInfo).SelectMany(x => x.GetCommanders().SelectMany(x => x.AlivePlaced)).Count(x => ShouldCount(x)) : 
            Commander.AlivePlaced.Count(x => ShouldCount(x));
        int amountDesired = Commander.AlivePlaced.Count(x => IsDesired(x));

        float currentRatio = (float)amountDesired / amountCounted;

        Dictionary<GameObject, float> weights = new Dictionary<GameObject, float>();
        foreach (GameObject obj in options)
        {
            bool isDesired = DesiredFilter.Check(obj);
            if (isDesired)
                weights.Add(obj, 1f - Mathf.Clamp01(currentRatio / DesiredRatio));
            else
                weights.Add(obj, NonDesiredWeight);
        }

        return weights;
    }

    private bool ShouldCount (Unit unit)
    {
        if (CountUseFactoryUnit && unit.TryGetComponent(out UnitFactory factory))
        {
            return CountFilter.Check(factory.UnitPrefab);
        }
        return CountFilter.Check(unit.gameObject);
    }

    private bool IsDesired (Unit unit)
    {
        if (DesiredUseFactoryUnit && unit.TryGetComponent(out UnitFactory factory))
        {
            return DesiredFilter.Check(factory.UnitPrefab);
        }
        return DesiredFilter.Check(unit.gameObject);
    }
}
