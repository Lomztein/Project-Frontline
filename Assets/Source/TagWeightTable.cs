using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tag Weight Table", menuName = "Unit Weight Tables/Tag")]
public class TagWeightTable : UnitGroupWeightTable
{
    public TagWeight[] Tags;

    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        float factor = Mathf.Clamp01(Commander.OffenseFactor);

        var weights = new Dictionary<GameObject, float>();
        foreach (var option in options)
        {
            var unit = option.GetComponent<Unit>();
            var tags = GetWeights(unit);
            if (tags.Any())
                weights.Add(option, tags.Max(x => x.Weight));
            else
                weights.Add(option, GetOtherWeight(factor));
        }

        return weights;
    }

    private IEnumerable<TagWeight> GetWeights(Unit unit)
    {
        foreach (var weight in Tags)
        {
            if (unit.Info.Tags.Contains(weight.Tag))
                yield return weight;
        }
    }

    [System.Serializable]
    public class TagWeight
    {
        public string Tag;
        public float Weight;
    }
}
