using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderApplier : MonoBehaviour
{
    public Commander Commander;

    void Start()
    {
        foreach (var comp in GetComponentsInChildren<ICommanderComponent>())
        {
            comp.AssignCommander(Commander);
        }
    }
}
