using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrioritizeTargetByFilterAIModifier : AIControllerModifier
{
    public GameObjectFilter Filter;
    public float Weight;

    public override void OnInitialized(AIController controller)
    {
        controller.AppendTargetEvaluator((x, y) =>
            Filter.Check(y) ? Weight : -Weight);    
    }
}
