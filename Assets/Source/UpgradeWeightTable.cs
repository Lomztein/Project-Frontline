using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade Weight Table", menuName = "Unit Weight Tables/Upgrade")]
public class UpgradeWeightTable : UnitWeightTable
{
    public float UnitsPerUpgradeStructure = 3;
    public float NonUpgradeWeight;

    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        var weights = new Dictionary<GameObject, float>();
        foreach (var option in options)
        {
            var upgraders = option.GetComponents<ChanceOnUnitSpawnUpgradeStructure>();
            if (upgraders.Length > 0) 
            {
                int affectedCount = upgraders.Sum(y => Commander.AliveProduced.Count(x => y.CanUpgrade(x.gameObject)));
                int currentCount = upgraders.Sum(y => Commander.AlivePlaced.Count(x => x.Info.Name == y.GetComponent<Unit>().Name));

                float desiredStructures = affectedCount / UnitsPerUpgradeStructure;
                weights.Add(option, 1f - Mathf.Clamp01(currentCount / desiredStructures));
            }
            else
            {
                weights.Add(option, NonUpgradeWeight);
            }
        }

        return weights;
    }
}
