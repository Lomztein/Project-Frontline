using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mutator : ScriptableObject
{
    public string Name;
    [TextArea]
    public string Description;

    public abstract void Start();

    public abstract void Stop();

    public static IEnumerable<Mutator> LoadMutators()
        => Resources.LoadAll<Mutator>("Mutators/");
}
