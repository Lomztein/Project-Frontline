using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPositionSeletor
{
    Vector3 SelectPosition(IEnumerable<Vector3> centers);
}
