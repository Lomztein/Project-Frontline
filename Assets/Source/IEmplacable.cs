using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEmplacable
{
    public void Emplace(Transform parent);

    public void Detatch();
}
