using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AI Player Profile", menuName = "AI Player Profile")]
public class AIPlayerProfile : ScriptableObject
{
    public const string PATH_TO_PROFILES = "AIProfiles/";
    public const string PATH_TO_DEFAULT = PATH_TO_PROFILES + "Default";

    public string Name;
    [TextArea]
    public string Description;

    public Vector2 SaveTimeMinMax = new Vector2(10, 40);
    public AnimationCurve SaveTimeBias = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public float ActionsPerMinute = 10;
    public float OffenseMargin = 150;

    [Header("Unit Weights")]
    public UnitWeightTableBase UnitWeightTable;
    public Vector2 WeightRandomizer;

    public static AIPlayerProfile[] LoadAll()
        => Resources.LoadAll<AIPlayerProfile>(PATH_TO_PROFILES);
}
