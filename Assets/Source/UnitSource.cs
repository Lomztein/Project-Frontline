using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitSource : MonoBehaviour
{
    public abstract GameObject[] GetAvailableUnitPrefabs();
}
