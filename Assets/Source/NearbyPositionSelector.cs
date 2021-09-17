using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NearbyPositionSelector : MonoBehaviour, IPositionSeletor
{
    public float PlacementSize;
    public float SearchStepSize;
    public float SearchStepSizeVariance;
    public int MaxIters;

    private const string StructTag = "StructureUnit";
    private const int TerrainLayer = 1 << 8;

    public Vector3 SelectPosition(IEnumerable<Vector3> centers, Vector3 checkSize)
    {
        Vector3 position = Vector3.zero;
        foreach (Vector3 center in centers)
        {
            int iters = 0;

            position = center;
            while (!CanPlace(position, checkSize) && iters < MaxIters)
            {
                Vector2 randBase = Random.insideUnitCircle;
                Vector3 rand = new Vector3(randBase.x, 0f, randBase.y);
                position += rand * (SearchStepSize + Random.Range(-SearchStepSizeVariance, SearchStepSizeVariance));
                iters++;
            }
        }

        return position;
    }

    private bool CanPlace (Vector3 position, Vector3 checkSize)
    {
        Collider[] colliders = Physics.OverlapBox(position, checkSize / 2f, Quaternion.identity, ~TerrainLayer);
        return !colliders.Any(x => x.CompareTag(StructTag));
    }
}
