using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureFocusAIControllerModifier : AIControllerModifier // Temp class, functionality should later be replaced with easily selectable target evaluator functions.
{
    private const string STRUCTURE_UNIT_TAG = "StructureUnit";

    public override void OnInitialized(AIController controller)
    {
        controller.SetTargetEvaluator(EvaluateTarget);
    }

    private float EvaluateTarget(Vector3 pos, GameObject obj)
    {
        float value = -Vector3.SqrMagnitude(pos - obj.transform.position) + (obj.CompareTag(STRUCTURE_UNIT_TAG) ? 10000000000 : -1000000000);
        return value;
    }
}
