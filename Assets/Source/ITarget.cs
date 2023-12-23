﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITarget
{
    bool IsValid();

    Vector3 GetCenter();

    Vector3 GetSize();

    GameObject GetGameObject();
}

public static class TargetExtensions
{
    public static bool ExistsAndValid (this ITarget target)
    {
        if (target == null)
        {
            return false;
        }
        return target.IsValid();
    }
}
