using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMouseAim : MonoBehaviour
{
    public Turret Turret;
    public LayerMask GroundLayer;

    void FixedUpdate()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, GroundLayer))
        {
            Turret.Target(hit.point, Time.fixedDeltaTime);
        }
    }
}
