using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitPurchasePredicate
{
    public bool CanPurchase(Unit unit, Commander commander);

    public string GetDescription(Unit unit, Commander commander);
}
