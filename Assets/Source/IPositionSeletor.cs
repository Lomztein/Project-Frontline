using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPositionSeletor
{
    IEnumerator SelectPosition(Commander commander, GameObject unit, OverlapUtils.OverlapShape shape, Action<Vector3> onPositionFound);
}
