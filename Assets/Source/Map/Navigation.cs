using System;
using System.Collections.Generic;
using UnityEngine;
using Util;
using C5;
using System.Collections;
using System.Linq;

public static class Navigation
{
    private static List<NavigationNode> _nodes = new List<NavigationNode>();

    public static IEnumerable<NavigationNode> GetPath(Vector3 start, Vector3 end)
    {
        NavigationNode startNode = GetNearestNode(start);
        NavigationNode endNode = GetNearestNode(end);
        return GetPath(startNode, endNode);
    }

    public static IEnumerable<NavigationNode> GetPath(NavigationNode start, NavigationNode end)
    {
        Dictionary<NavigationNode, bool> visited = new Dictionary<NavigationNode, bool>(_nodes.ToDictionary(x => x, y => false));
        Dictionary<NavigationNode, NavigationNode> path = new Dictionary<NavigationNode, NavigationNode>(_nodes.ToDictionary(x => x, new Func<NavigationNode, NavigationNode>(y => null)));
        IntervalHeap<NavigationNode> toExplore = new IntervalHeap<NavigationNode>(new NodeHeuristic(end))
        {
            start
        };

        NavigationNode finalNode = null;
        while (finalNode == null && toExplore.Count > 0)
        {
            finalNode = Explore(toExplore, end, visited, path);
        }

        List<NavigationNode> result = new List<NavigationNode>();
        while (finalNode != null) // butt
        {
            result.Add(finalNode);
            finalNode = path[finalNode];
        }
        result.Reverse();
        return result;
    }

    private static NavigationNode Explore(IntervalHeap<NavigationNode> toExplore, NavigationNode endNode, Dictionary<NavigationNode, bool> visited, Dictionary<NavigationNode, NavigationNode> path)
    {
        NavigationNode next = toExplore.DeleteMax();
        visited[next] = true;
        return Expand(toExplore, next, endNode, visited, path);
    }

    private static NavigationNode Expand(IntervalHeap<NavigationNode> toExplore, NavigationNode node, NavigationNode endNode, Dictionary<NavigationNode, bool> visited, Dictionary<NavigationNode, NavigationNode> path)
    {
        foreach (NavigationNode neighbour in node.Neighbours)
        {
            if (visited[neighbour])
                continue;

            path[neighbour] = node;
            if (neighbour == endNode)
                return neighbour;

            visited[neighbour] = true;
            toExplore.Add(neighbour);
        }

        return null;
    }

    public static NavigationNode GetNearestNode(Vector3 position)
        => UnityUtils.FindBest(_nodes, x => -(position - x.Position).sqrMagnitude);

    public static void AddNode(NavigationNode node)
    {
        _nodes.Add(node);
    }

    public static void RemoveNode(NavigationNode node)
    {
        _nodes.Remove(node);
    }

    public static void ClearNodes()
    {
        foreach (var node in _nodes)
        {
            UnityEngine.Object.Destroy(node.gameObject);
        }
        _nodes.Clear();
    }

    public static Vector3 IncomingVector(NavigationNode prev, NavigationNode next)
        => (prev.Position - next.Position).normalized;

    public static Vector3 OutgoingVector(NavigationNode prev, NavigationNode next)
        => (next.Position - prev.Position).normalized;

    public static float IncomingAngle(NavigationNode prev, NavigationNode next)
    {
        Vector3 incoming = IncomingVector(prev, next);
        return Mathf.Atan2(incoming.x, incoming.z) * Mathf.Rad2Deg;
    }

    public static float OutgoingAngle(NavigationNode prev, NavigationNode next)
    {
        Vector3 outgoing = OutgoingVector(prev, next);
        return Mathf.Atan2(outgoing.x, outgoing.z) * Mathf.Rad2Deg;
    }

    public static (NavigationNode prev, NavigationNode next) GetSorroundingNodes(NavigationNode[] waypoints, int index)
    {
        if (index == waypoints.Length - 1)
            return (waypoints[index], null);
        return (waypoints[index], waypoints[index + 1]);
    }

    private class NodeHeuristic : IComparer<NavigationNode>
    {
        private NavigationNode _endNode;

        public NodeHeuristic(NavigationNode endNode) {
            _endNode = endNode;
        }

        public int Compare(NavigationNode x, NavigationNode y)
        {
            float us = (_endNode.Position - x.Position).sqrMagnitude;
            float them = (_endNode.Position - y.Position).sqrMagnitude;
            return Comparer.Default.Compare(them, us);
        }
    }
}
