using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitSelector
{
    GameObject SelectUnit(IEnumerable<GameObject> options);
}
