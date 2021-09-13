using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureFocusAIControllerModifier : AIControllerModifier // Temp class, functionality should later be replaced with easily selectable target evaluator functions.
{
    private const string STRUCTURE_UNIT_TAG = "StructureUnit";

    public override void OnInitialized(AIController controller)
    {
        controller.SetTargetEvaluator((pos, go) => -Vector3.SqrMagnitude(pos - transform.position) + (go.CompareTag(STRUCTURE_UNIT_TAG) ? 10000000 : 0));
    }
}
