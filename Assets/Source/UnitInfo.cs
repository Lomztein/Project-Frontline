using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitInfo
{
    public enum Role
    {
        Vanguard, Support, Defense, Production
    }

    public enum Tier
    {
        Basic, Intermediate, Advanced, Omega
    }

    public enum Type
    {
        Infantry, Vehicle, Aircraft, Defense, Structure, Naval
    }

    public string Identifier;
    public string Name;
    [TextArea]
    public string ShortDescription;
    [TextArea]
    public string Description;
    public string[] Tags;
    public int Cost;
    public int Value;

    public Role UnitRole;
    public Tier UnitTier;
    public Type UnitType;
}
