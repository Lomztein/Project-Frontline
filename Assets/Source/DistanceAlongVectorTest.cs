using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceAlongVectorTest : MonoBehaviour
{
    public void OnDrawGizmos()
    {
        Vector3 differenceDirection = transform.forward;
        float difference = 0;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            difference = Vector3.Dot(differenceDirection,
                hit.point - transform.position);
        }

        Gizmos.DrawLine(transform.position, transform.forward * difference);
    }
}
