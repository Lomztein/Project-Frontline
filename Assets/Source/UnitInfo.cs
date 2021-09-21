﻿using System.Collections;
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

    public enum Class
    {
        InfantrySupport, MainBattleTank, Sentry, Artillery, HeavyTank, Destroyer, AssaultInfantry, SupportInfantry
    }

    public enum Type
    {
        Infantry, Vehicle, Aircraft, Defense, Structure
    }

    public string Name;
    [TextArea]
    public string Description;
    public int Cost;
    public int Value;

    public Role UnitRole;
    public Tier UnitTier;
    public Class UnitClass;
    public Type UnitType;
}
