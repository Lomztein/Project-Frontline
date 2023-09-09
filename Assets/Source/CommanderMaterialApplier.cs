using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderMaterialApplier : MonoBehaviour, ICommanderComponent
{
    [SerializeField] private Renderer[] _renderersToApplyTo;

    public void AssignCommander(Commander commander)
    {
        foreach (Renderer renderer in _renderersToApplyTo)
        {
            renderer.material = commander.UnitPalette.UnitBodyMaterial;
        }
        //StartCoroutine(WaitAndPaint(commander));
    }

    private IEnumerator WaitAndPaint(Commander cmd)
    {
        yield return new WaitForEndOfFrame();
        foreach (Renderer renderer in _renderersToApplyTo)
        {
            renderer.material = cmd.UnitPalette.UnitBodyMaterial;
        }
    }
}
