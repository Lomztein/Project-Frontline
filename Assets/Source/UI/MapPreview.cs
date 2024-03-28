using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Util;

public class MapPreview : MonoBehaviour
{
    public UIPolygon Polygon;
    public GameObject SpawnPointPrefab;
    public Transform SpawnPointParent;
    public RectTransform BoundsIndicator;
    public float BoundsMargin = 2f;
    public TMP_Text WidthText;
    public TMP_Text HeightText;

    private void Start()
    {
        MatchSetup.OnUpdated += MatchSettings_OnUpdated;
        MatchSettings_OnUpdated(MatchSetup.Current);
    }

    private void OnDestroy()
    {
        MatchSetup.OnUpdated -= MatchSettings_OnUpdated;
    }

    private IEnumerator DelayedUpdate(MatchSetup obj)
    {
        yield return new WaitForEndOfFrame();
        IEnumerable<Vector2> parimiter = obj.MapInfo.GetPerimeterPolygon()
            .Select(x => new Vector2(x.x, x.z)).ToArray();
        float max = parimiter.Max(x => Mathf.Max(x.x, x.y));
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
            newSpawnPoint.transform.localPosition = new Vector3(-position.y, position.x);
            newSpawnPoint.GetComponentInChildren<TMP_Text>().text = i.ToString();
        }

        BoundsIndicator.sizeDelta = new Vector2(parimiter.Max(x => x.y), parimiter.Max(x => x.x)) * Polygon.Size + Vector2.one * BoundsMargin;

        WidthText.text = obj.MapInfo.Bounds.size.z.ToString("F0", CultureInfo.InvariantCulture);
        HeightText.text = obj.MapInfo.Bounds.size.x.ToString("F0", CultureInfo.InvariantCulture);

        Polygon.DrawPolygon(parimiter.Reverse().ToArray());
    }

    private void MatchSettings_OnUpdated(MatchSetup obj)
    {
        StartCoroutine(DelayedUpdate(obj));
    }
}
