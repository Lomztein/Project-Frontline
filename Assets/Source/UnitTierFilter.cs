using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Tier Filter", menuName = "GameObject Filter/Unit Tier Filter")]
public class UnitTierFilter : UnitFilter
{
    public UnitInfo.Tier[] AllowedTiers;

    protected override bool Check(Unit unit)
        => AllowedTiers.Contains(unit.Info.UnitTier);
}
