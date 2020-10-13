using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionObject : MonoBehaviour
{
    public Faction Faction { get; private set; }

    public void SetFaction (Faction faction)
    {
        Faction = faction;

        foreach (var transform in GetComponentsInChildren<Transform>())
        {
            transform.gameObject.layer = faction.GetLayer();
        }

        var components = GetComponentsInChildren<IFactionComponent>();
        foreach (var component in components)
        {
            component.SetFaction(faction);
        }
    }
}
