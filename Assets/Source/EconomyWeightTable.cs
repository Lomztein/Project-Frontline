using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Economy Weight Table", menuName = "Unit Weight Tables/Economy")]
public class EconomyWeightTable : UnitWeightTable
{
    private const string ECONOMY_TAG = "Economy";

    public int BaseIncomeTarget = 50;
    public float IncomeTargetPerMinute = 20f;
    public float NonEcoUnitWeight = 0f;

    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        float currentIncome = Commander.AverageIncomePerSecond;
        float targetIncome = BaseIncomeTarget + IncomeTargetPerMinute * MatchController.MatchTime;
       
        Dictionary<GameObject, float> weights = new Dictionary<GameObject, float>();
        foreach (var option in options)
        {
            if (option.GetComponent<Unit>().Info.Tags.Contains(ECONOMY_TAG))
            {
                weights.Add(option, 1f - Mathf.Clamp01(currentIncome / targetIncome));
            }else
            {
                weights.Add(option, NonEcoUnitWeight);
            }
        }

        return weights;
    }
}
