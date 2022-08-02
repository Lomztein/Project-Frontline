using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneryGeneratorStep
{
    public void Execute(MapInfo info);
}
