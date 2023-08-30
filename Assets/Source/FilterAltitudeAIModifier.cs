using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterAltitudeAIModifier : AIControllerModifier
{
    public Vector2 AltitudeMinMax;

    public override void OnInitialized(AIController controller)
    {
        controller.AppendTargetFilter(x => x.transform.position.y > AltitudeMinMax.x && x.transform.position.y < AltitudeMinMax.y);
    }
}
