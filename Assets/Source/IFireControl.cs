using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFireControl
{
    void Fire(int amount, Action<int> callback);
}
