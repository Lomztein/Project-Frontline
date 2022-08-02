using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLine
{
    public Vector3 Start;
    public Vector3 End;

    public SpawnLine (Vector3 start, Vector3 end)
    {
        Start = start;
        End = end;
    }

    public Vector3 GetSpawnPoint(int index, int total)
    {
        if (total == 1) return Vector3.Lerp(Start, End, 0.5f);
        float val = (float)index / (total - 1);
        return Vector3.Lerp(Start, End, val);
    }
}
