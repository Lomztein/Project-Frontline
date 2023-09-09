using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitCostModifier
{
    public int Modify(int cost, Unit unit, Commander commander);

    public string GetDescription(int cost, Unit unit, Commander commander);
}
