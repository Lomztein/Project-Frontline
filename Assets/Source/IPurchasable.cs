using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPurchasable
{
    string Name { get; }
    string Description { get; }
    Sprite Sprite { get; }
    int GetCost(Commander commander);
}
