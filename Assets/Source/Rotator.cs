using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 RotateSpeed;

    void FixedUpdate()
    {
        transform.Rotate(RotateSpeed * Time.fixedDeltaTime);
    }
}
