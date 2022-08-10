using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

public class WeightedUnitSelector : MonoBehaviour, IUnitSelector
{
    public UnitWeightTableBase WeightTable;
    public Vector2 WeightRandomizer = Vector2.one;
    private bool _initialized;

    public Text WeightDebug;

    private void Start()
    {
        WeightTable = WeightTable.DeepCopy();
    }

    public GameObject SelectUnit(IEnumerable<GameObject> options)
    {
        if (!_initialized)
        {
            Commander commander = GetComponent<Commander>();
            WeightTable.Initialize(commander, commander.UnitSource.GetAvailableUnitPrefabs(commander.Faction));
            _initialized = true;
        }

        var tableWeights = WeightTable.GetWeights(options);
        var randomWeights = options.Select(x => tableWeights[x] * UnityEngine.Random.Range(WeightRandomizer.x, WeightRandomizer.y));
        var zip = randomWeights.Zip(options, (weight, option) => new { weight, option });


        float highestWeight = float.MinValue;
        GameObject highestUnit = null;
        foreach (var entry in zip)
        {
            if (entry.weight < 0.000001f) // Weight of 0 means don't.
                continue;

            if (entry.weight > highestWeight)
            {
                highestWeight = entry.weight;
                highestUnit = entry.option;
            }
        }

        UpdateWeightDebug(tableWeights, highestUnit);

        return highestUnit;
    }

    private void UpdateWeightDebug (Dictionary<GameObject, float> weights, GameObject selected)
    {
        if (WeightDebug && weights.Count > 0)
        {
            var values = weights.Values.ToArray();
            var keys = weights.Keys.ToArray();
            Array.Sort(values, keys);

            Array.Reverse(values);
            Array.Reverse(keys);

            WeightDebug.text = string.Empty;
            for (int i = 0; i < values.Length; i++)
            {
                if (keys[i] == selected)
                {
                    WeightDebug.text += "<color=red><b>" + keys[i].name + ": " + values[i] + "</b></color>\n";
                }
                else
                {
                    WeightDebug.text += keys[i].name + ": " + values[i] + "\n";
                }
            }
        }
    }
}
