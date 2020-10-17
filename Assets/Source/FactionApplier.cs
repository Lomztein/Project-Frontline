using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionApplier : MonoBehaviour
{
    public Faction Faction;
    public void Start()
    {
        Faction.ApplyFaction(gameObject);
    }

}
