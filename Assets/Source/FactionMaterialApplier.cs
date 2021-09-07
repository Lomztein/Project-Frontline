using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionMaterialApplier : MonoBehaviour, IFactionComponent
{
    [SerializeField] private Renderer[] _renderersToApplyTo;

    public void SetFaction(Faction faction)
    {
        foreach (Renderer renderer in _renderersToApplyTo)
        {
            renderer.material = faction.FactionMaterial;
        }
    }
}
