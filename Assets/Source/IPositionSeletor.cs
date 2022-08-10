using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPositionSeletor
{
    Vector3? SelectPosition(Commander commander, GameObject unit, Vector3 checkSize);
}
