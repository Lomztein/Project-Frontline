using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnamentPlayerNode
{
    public PlayerInfo PlayerInfo;

    public override string ToString()
    {
        return $"PlayerNode: {PlayerInfo?.ToString() ?? "empty"}";
    }
}
