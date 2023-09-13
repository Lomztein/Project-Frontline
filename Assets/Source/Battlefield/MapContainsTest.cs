using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapContainsTest : MonoBehaviour
{
    public Vector3 StartPos;

    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            float dist = MatchSettings.Current.MapInfo.DistanceToEdge(hit.point);
            Debug.DrawRay(hit.point, hit.normal * Mathf.Abs(dist), MatchSettings.Current.MapInfo.Contains(hit.point) ? Color.green : Color.red);
            NavigationNode[] navNodes = Navigation.GetPath(StartPos, hit.point).ToArray();
            for (int i = 0; i < navNodes.Length - 1; i++)
            {
                Debug.DrawLine(navNodes[i].Position + Vector3.up * (i + 5), navNodes[i + 1].Position + Vector3.up * (i + 5), Color.black);
            }
        }
    }
}
