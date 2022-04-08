using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Type Filter", menuName = "GameObject Filter/Unit Type Filter")]
public class UnitTypeFilter : UnitFilter
{
    public UnitInfo.Type[] AllowedTypes;

    protected override bool Check(Unit unit)
        => AllowedTypes.Contains(unit.Info.UnitType);
}
