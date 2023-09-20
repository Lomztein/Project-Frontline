using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeProperty : PropertyBase
{
    public float Min;
    public float Max;
    public bool IntegersOnly;

    public RangeProperty(string name, string description, float min, float max, bool integersOnly) : base(name, description)
    {
        Min = min;
        Max = max;
        IntegersOnly = integersOnly;
    }
}
