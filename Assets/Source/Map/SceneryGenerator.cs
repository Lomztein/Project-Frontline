using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

[CreateAssetMenu(fileName = "New Scenery Generator", menuName = "Scenery Generator")]
public class SceneryGenerator : ScriptableObject
{
    public string Name;
    public string Description;

    [SerializeReference, SR]
    public ISceneryGeneratorStep[] Steps;

    public void Generate (MapInfo info)
    {
        foreach (var step in Steps)
        {
            step.Execute(info);
        }
    }
}
