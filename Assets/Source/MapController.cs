using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public SceneryGenerator SceneryGenerator;

    public void Start()
    {
        SceneryGenerator.Generate(MatchSetup.Current.MapInfo);
    }
}
