using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitComparer : IComparer<GameObject>
{
    public int Compare(GameObject x, GameObject y)
    {
        Unit lhsu = x.GetComponent<Unit>();
        Unit rhsu = y.GetComponent<Unit>();

        if (lhsu.Info.UnitTier == rhsu.Info.UnitTier)
            return lhsu.Cost - rhsu.Cost;
        return lhsu.Info.UnitTier - rhsu.Info.UnitTier;
    }
}
