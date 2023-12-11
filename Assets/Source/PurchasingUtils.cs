using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PurchasingUtils
{
    public static int GetCost(Unit unit, int baseCost, IEnumerable<IUnitCostModifier> modifiers, Commander commander)
    {
        int cost = baseCost;
        foreach (var modifier in modifiers)
        {
            cost = modifier.Modify(cost, unit, commander);
        }
        return cost;
    }

    public static string GetCostModifierDesription(Unit unit, int baseCost, IEnumerable<IUnitCostModifier> modifiers, Commander commander)
        => string.Join("\n", modifiers.Select(x => x.GetDescription(baseCost, unit, commander)).Where(x => !string.IsNullOrWhiteSpace(x))).Trim();

    public static bool CanPurchase(Unit unit, IEnumerable<IUnitPurchasePredicate> predicates, Commander commander)
    {
        var any = predicates.Where(x => x.Behaviour == IUnitPurchasePredicate.AnyAll.Any);
        var all = predicates.Where(x => x.Behaviour == IUnitPurchasePredicate.AnyAll.All);
        return all.All(x => x.CanPurchase(unit, commander)) && (!any.Any() || any.Any(x => x.CanPurchase(unit, commander)));
    }

    public static string GetCanPurchaseDesription(Unit unit, IEnumerable<IUnitPurchasePredicate> predicates, Commander commander)
        => string.Join("\n", predicates.Select(x => x.GetDescription(unit, commander)).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()).Trim();
}
