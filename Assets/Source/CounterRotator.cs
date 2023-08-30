using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterRotator : MonoBehaviour
{
    public Transform Target;
    public Vector3 Multipliers = Vector3.one;

    private void Update()
    {
        Vector3 eulerRot = Target.eulerAngles;
        eulerRot = new Vector3(
            eulerRot.x * Multipliers.x,
            eulerRot.y * Multipliers.y,
            eulerRot.z * Multipliers.z
            );
        transform.localRotation = Quaternion.Euler(eulerRot);
    }
}
