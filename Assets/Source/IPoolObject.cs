using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject
{
    bool IsAvailable { get; }
    GameObject GameObject { get; }

    void OnInstantiated();
    void OnEnabled();
    void Dispose();
}
