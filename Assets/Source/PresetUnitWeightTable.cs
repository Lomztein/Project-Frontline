using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Preset Weight Table", menuName = "Unit Weight Tables/Preset")]
public class PresetUnitWeightTable : UnitWeightTableBase
{
    public Preset[] Presets;

    public override UnitWeightTableBase DeepCopy()
    {
        PresetUnitWeightTable table = Instantiate(this);
        table.Presets = Presets;
        return table;
    }

    public override Dictionary<GameObject, float> GetWeights(IEnumerable<GameObject> options)
    {
        var result = new Dictionary<GameObject, float>();
        foreach (var preset in Presets)
        {
            if (options.Contains(preset.Unit))
            {
                result.Add(preset.Unit, preset.Weight);
            }
        }
        return result;
    }

    public override void Initialize(Commander commander, IEnumerable<GameObject> availableUnits)
    {
    }

    [System.Serializable]
    public class Preset
    {
        public GameObject Unit;
        public float Weight;
    }
}
