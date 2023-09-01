using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ratio Weight Table", menuName = "Unit Weight Tables/Ratio")]
public class UnitRatioWeightTable : UnitWeightTable
{
    public UnitFilter CountFilter;
    public UnitFilter DesiredFilter;

    public bool CountFriendlyPlaced;
    public bool CountFriendlyAlive;

    public bool CountEnemyPlaced;
    public bool CountEnemyAlive;

    public bool CountUseFactoryUnit;
    public bool DesiredUseFactoryUnit;

    public float DesiredRatio;
    public int Margin = 10;
    public int Offset = 10;

    public float NonDesiredWeight;



    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        int currentAmount = 0;

        if (CountFriendlyPlaced)
            currentAmount += Commander.AlivePlaced.Count(x => ShouldCount(x));
        if (CountFriendlyAlive)
            currentAmount += Commander.AliveProduced.Count(x => ShouldCount(x));

        if (CountEnemyPlaced || CountEnemyAlive)
        {
            var enemyCommanders = Team.GetOtherTeams(Commander.TeamInfo).SelectMany(x => x.GetCommanders());
            if (CountEnemyPlaced)
                currentAmount += enemyCommanders.SelectMany(x => x.AlivePlaced).Count(x => ShouldCount(x));
            if (CountEnemyAlive)
                currentAmount += enemyCommanders.SelectMany(x => x.AliveProduced).Count(x => ShouldCount(x));
        }
            
        int currentDesiredAmount = Commander.AlivePlaced.Count(x => IsDesired(x));

        Dictionary<GameObject, float> weights = new Dictionary<GameObject, float>();
        foreach (GameObject obj in options)
        {
            bool isDesired = DesiredFilter.Check(obj);
            if (isDesired)
                weights.Add(obj, CalculateDesire(currentDesiredAmount, currentAmount, DesiredRatio, Margin, Offset));
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
