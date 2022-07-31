using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AI Player Profile", menuName = "AI Player Profile")]
public class AIPlayerProfile : ScriptableObject
{
    public const string PATH_TO_PROFILES = "AI Player Profiles/";
    public const string PATH_TO_DEFAULT = PATH_TO_PROFILES + "Default";

    public string Name;
    [TextArea]
    public string Description;

    public float ActionsPerMinute;

    [Header("Unit Weights")]
    public UnitWeightTableBase UnitWeightTable;
    public Vector2 WeightRandomizer;
}
