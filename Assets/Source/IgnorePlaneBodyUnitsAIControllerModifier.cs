using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnorePlaneBodyUnitsAIControllerModifier : AIControllerModifier
{
    public override void OnInitialized(AIController controller)
    {
        controller.AppendTargetFilter(x => x.transform.root.GetComponent<PlaneBody>() == null);
    }
}
