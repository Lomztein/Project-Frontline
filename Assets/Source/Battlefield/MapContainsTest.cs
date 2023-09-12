using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapContainsTest : MonoBehaviour
{
    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            float dist = MatchSettings.Current.MapInfo.DistanceToEdge(hit.point);
            Debug.DrawRay(hit.point, hit.normal * Mathf.Abs(dist), MatchSettings.Current.MapInfo.Contains(hit.point) ? Color.green : Color.red);
        }
    }
}
