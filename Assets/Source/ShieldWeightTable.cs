using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shield Weight Table", menuName = "Unit Weight Tables/Shield")]
public class ShieldWeightTable : UnitWeightTable
{
    public float NonShieldWeight;

    public float TargetCoverage = 1.2f;

    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        var projectors = Commander.AlivePlaced.Select(x => x.GetComponentInChildren<ShieldProjector>()).Where(x => x != null);
        int covers = Commander.AlivePlaced.Sum(x => GetNumCovers(x, projectors));
        float coverage = (float)covers / Commander.AlivePlaced.Count();

        Dictionary<GameObject, float> weights = new Dictionary<GameObject, float>();
        foreach (var option in options)
        {
            ShieldProjector projector = option.GetComponent<ShieldProjector>();
            if (projector != null)
            {
                weights.Add(option, 1f - Mathf.Clamp01(coverage / TargetCoverage));
            }
            else
            {
                weights.Add(option, NonShieldWeight);
            }
        }

        return weights;
    }

    private int GetNumCovers (Unit obj, IEnumerable<ShieldProjector> projectors)
    {
        int num = projectors.Count(y => Vector3.SqrMagnitude(obj.transform.position - y.transform.position) < Mathf.Pow(y.ShieldSize / 2f, 2f));
        return num;
    }
}
