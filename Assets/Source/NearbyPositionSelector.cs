using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NearbyPositionSelector : MonoBehaviour, IPositionSeletor
{
    public float PlacementSize;
    public float SearchStepSize;
    public int MaxIters;

    private const string StructTag = "StructureUnit";
    private const int TerrainLayer = 1 << 8;

    private Vector3[] _searchDirections = new Vector3[]
    {
        new Vector3 (1f, 0f, 0f),
        new Vector3 (0f, 0f, 1f),
        new Vector3 (-1f, 0f, 0f),
        new Vector3 (0f, 0f, -1f)
    };

    public Vector3 SelectPosition(IEnumerable<Vector3> centers)
    {
        Vector3 position = Vector3.zero;
        foreach (Vector3 center in centers)
        {
            int iters = 0;

            position = center;
            while (!CanPlace(position) && iters < MaxIters)
            {
                position += _searchDirections[Random.Range(0, _searchDirections.Length)] * SearchStepSize;
                iters++;
            }
        }

        return position;
    }

    private bool CanPlace (Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, PlacementSize, ~TerrainLayer);
        return !colliders.Any(x => x.CompareTag(StructTag));
    }
}
