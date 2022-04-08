using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Role Filter", menuName = "GameObject Filter/Unit Role Filter")]
public class UnitRoleFilter : UnitFilter
{
    public UnitInfo.Role[] AllowedRoles;

    protected override bool Check(Unit unit)
        => AllowedRoles.Contains(unit.Info.UnitRole);
}