using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLine : ISpawnVolume
{
    public Vector3 Center;
    public Vector3 ExtendDirection;

    public float DistanceBetween;

    public int Max => int.MaxValue;
    public Vector3 Position => Center;

    public SpawnLine (Vector3 center, Vector3 extendDirection, float distanceBetween)
    {
        Center = center;
        ExtendDirection = extendDirection;
        DistanceBetween = distanceBetween;
    }

    public Vector3 GetSpawnPoint(int index, int total)
    {
        // shrug
        Vector3 startPos = Center - ExtendDirection * (DistanceBetween/2f * (total - 1f)) * 2f;
        return startPos + ExtendDirection * DistanceBetween * index * 2f;
    }
}
