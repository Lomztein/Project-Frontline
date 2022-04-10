using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Identifier Filter", menuName = "GameObject Filter/Unit Identifier Filter")]
public class UnitIdentifierFilter : UnitFilter
{
    public string[] AllowedIdentifiers;

    protected override bool Check(Unit unit)
        => AllowedIdentifiers.Any(x => unit.Info.Identifier.StartsWith(x));
}
