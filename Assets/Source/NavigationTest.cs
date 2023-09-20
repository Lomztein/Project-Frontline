using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavigationTest : MonoBehaviour
{
    public int GridHeight;
    public int GridWidth;
    public float GridSize;
    public LayerMask CheckLayer;

    public Vector2Int[] Nearby = new Vector2Int[]
        {
             new Vector2Int(1, 0),
             new Vector2Int(1, 1),
             new Vector2Int(0, 1),
             new Vector2Int(-1, 1),
             new Vector2Int(-1, 0),
             new Vector2Int(-1, -1),
             new Vector2Int(0, -1),
             new Vector2Int(1, -1),
        };
    
    private void Start()
    {
        BuildGrid(GridWidth, GridHeight, GridSize);
    }

    void BuildGrid(int width, int height, float size)
    {
        NavigationNode[,] nodes = new NavigationNode[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = transform.position + new Vector3(x * size, 0f, y * size);
                if (!Physics.CheckSphere(position, size / 2f, CheckLayer))
                {
                    nodes[x, y] = NavigationNode.Create(position, new NavigationNode[Nearby.Length]);
                }
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                NavigationNode node = nodes[x, y];
                if (node != null)
                {
                    var neighbours = Nearby.Select(v => GetNode(v.x + x, v.y + y, nodes)).Where(v => v != null);
                    node.Neighbours = neighbours.ToArray();
                }
            }
        }
    }

    private static bool IsInsideGrid(int x, int y, int width, int height)
    {
        if (x < 0 || y < 0) return false;
        if (x > width - 1 || y > height - 1) return false;
        return true;
    }

    private NavigationNode GetNode(int x, int y, NavigationNode[,] nodes)
    {
        if (IsInsideGrid(x, y, nodes.GetLength(0), nodes.GetLength(1)))
            return nodes[x, y];
        else return null;
    }
}
