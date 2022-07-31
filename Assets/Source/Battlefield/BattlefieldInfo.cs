using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

[System.Serializable]
public class BattlefieldInfo
{
    public float Width;
    public float Height;

    [SerializeReference, SR]
    public IBattlefieldShape Shape;
    public SceneryGenerator SceneryGenerator;

    public Vector2 Size => new Vector2(Height, Width);
}
