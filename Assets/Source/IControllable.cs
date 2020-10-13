using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControllable
{
    Vector3 Accelerate(float factor);

    Vector3 Turn(float factor);
}
