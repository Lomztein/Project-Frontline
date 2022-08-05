using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Economy Weight Table", menuName = "Unit Weight Tables/Economy")]
public class EconomyWeightTable : UnitWeightTable
{
    private const string ECONOMY_TAG = "Economy";

    public int BaseEconomyUnitsTarget = 2;
    public float EconomyUnitsPerMinute = 0.3f;
    public float NonEcoUnitWeight = 0f;

    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        int economyUnitCount = Commander.AlivePlaced.Count(x => x.Info.Tags.Contains(ECONOMY_TAG));
        float targetCount = BaseEconomyUnitsTarget + (EconomyUnitsPerMinute * MatchController.MatchTime / 60f);
       
        Dictionary<GameObject, float> weights = new Dictionary<GameObject, float>();
        foreach (var option in options)
        {
            if (option.GetComponent<Unit>().Info.Tags.Contains(ECONOMY_TAG))
            {
                weights.Add(option, 1f - Mathf.Clamp01(economyUnitCount / targetCount));
            }else
            {
                weights.Add(option, NonEcoUnitWeight);
            }
        }

        return weights;
    }
}
