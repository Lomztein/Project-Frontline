using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPositionEvaluator
{
    public float Evaluate(Commander commander, GameObject unit, Vector3 position);
}
