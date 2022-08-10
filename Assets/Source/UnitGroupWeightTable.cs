using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitGroupWeightTable : UnitWeightTable
{
    public enum OtherWeights { Zero, One, Inverse }
    public OtherWeights Others;

    public float GetOtherWeight(float factor)
    {
        switch (Others)
        {
            case OtherWeights.Zero: return 0;
            case OtherWeights.One: return 1;
            case OtherWeights.Inverse: return 1 - factor;
                default: return 0;
        }
    }
}
