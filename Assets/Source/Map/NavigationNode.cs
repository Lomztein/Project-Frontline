using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NavigationNode : MonoBehaviour
{
    private const string RESOURCE_PATH = "Prefabs/Waypoints/NavigationNode";

    public Vector3 Position => transform.position;
    public NavigationNode[] Neighbours;

    private void Awake()
    {
        Navigation.AddNode(this);
    }

    private void OnDestroy()
    {
        Navigation.RemoveNode(this);
    }

    private void OnDrawGizmos()
    {
        foreach (var node in Neighbours)
        {
            Gizmos.DrawLine(Position, node.Position);
        }
    }

    public static NavigationNode Create(Vector3 position, NavigationNode[] neighbours)
    {
        GameObject newNode = Instantiate(Resources.Load<GameObject>(RESOURCE_PATH), position, Quaternion.identity);
        newNode.transform.position = position;
        NavigationNode node = newNode.GetComponent<NavigationNode>();
        node.Neighbours = neighbours;
        return node;
    }
}
