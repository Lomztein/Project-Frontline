using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitPurchasePredicate
{
    public enum AnyAll { Any, All }
    public AnyAll Behaviour { get; } 

    public bool CanPurchase(Unit unit, Commander commander);

    public string GetDescription(Unit unit, Commander commander);
}
