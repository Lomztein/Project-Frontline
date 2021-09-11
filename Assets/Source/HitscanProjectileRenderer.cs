using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HitscanProjectileRenderer : MonoBehaviour
{
    public abstract void SetPositions(Vector3 start, Vector3 end);
}
