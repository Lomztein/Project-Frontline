using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Tag Filter", menuName = "GameObject Filter/Unit Tag Filter")]
public class UnitTagFilter : UnitFilter
{
    public string[] AllowedTags;

    protected override bool Check(Unit unit)
        => AllowedTags.Any(x => unit.Info.Tags.Any(y => y.StartsWith(x)));
}
