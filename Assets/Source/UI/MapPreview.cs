using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Util;

public class MapPreview : MonoBehaviour
{
    public UIPolygon Polygon;
    public GameObject SpawnPointPrefab;
    public Transform SpawnPointParent;

    private void Start()
    {
        MatchSettings.OnUpdated += MatchSettings_OnUpdated;
        MatchSettings_OnUpdated(MatchSettings.Current);
    }

    private void OnDestroy()
    {
        MatchSettings.OnUpdated -= MatchSettings_OnUpdated;
    }

    private IEnumerator DelayedUpdate(MatchSettings obj)
    {
        yield return new WaitForEndOfFrame();
        IEnumerable<Vector2> parimiter = obj.MapInfo.GetPerimeterPolygon()
            .Select(x => new Vector2(x.x, x.z)).ToArray();
        float max = parimiter.Max(x => x.magnitude);
        parimiter = parimiter.Select(x => x / max);
        parimiter = Enumerable.Concat(parimiter, parimiter.First().ObjectToEnumerable());

        foreach (Transform child in SpawnPointParent)
        {
            Destroy(child.gameObject);
        }

        var spawns = obj.MapInfo.Shape.GenerateSpawnVolumes(obj.MapInfo).Select(x => new Vector2(x.Position.x, x.Position.z)).ToArray();
        for (int i = 0; i < spawns.Length; i++)
        {
            Vector2 position = spawns[i] / max * Polygon.Size / 2f;
            var newSpawnPoint = Instantiate(SpawnPointPrefab, SpawnPointParent);
            newSpawnPoint.transform.localPosition = position;
            newSpawnPoint.GetComponentInChildren<TMP_Text>().text = i.ToString();
        }

        Polygon.DrawPolygon(parimiter.Reverse().ToArray());
    }

    private void MatchSettings_OnUpdated(MatchSettings obj)
    {
        StartCoroutine(DelayedUpdate(obj));
    }
}
