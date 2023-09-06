using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class Hitmarker : MonoBehaviour
{
    private const string RESOURCE = "Prefabs/UI/Hitmarker";
    private const string CANVAS = "Canvas";

    public Gradient ColorByDamageDone;
    public Vector2 DamageDoneMinMax = new Vector2(0.5f, 2.0f);
    public float BaseAlpha = 0.75f;
    public float FadeTime = 0.2f;
    public Image Image;

    private Vector3 _worldPos;

    public static Hitmarker Create(Vector3 worldPosition, float baseDamage, float damageDone)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        GameObject prefab = Resources.Load<GameObject>(RESOURCE);
        GameObject go = Instantiate(prefab, screenPos, Quaternion.identity);
        go.transform.SetParent(UnityUtils.MainCanvas.transform, true);
        Hitmarker marker = go.GetComponent<Hitmarker>();
        float factor = damageDone / baseDamage;
        float t = Mathf.InverseLerp(marker.DamageDoneMinMax.x, marker.DamageDoneMinMax.y, factor);
        marker.Image.color = marker.ColorByDamageDone.Evaluate(t);
        marker._worldPos = worldPosition;
        marker.StartCoroutine(marker.Fade());
        return marker;
    }

    private IEnumerator Fade()
    {
        float a = Image.color.a;
        while (a > 0f) {
            var color = Image.color;
            color.a = a * BaseAlpha;
            Image.color = color;
            Vector3 pos = Camera.main.WorldToScreenPoint(_worldPos);
            transform.position = pos;
            a -= Time.deltaTime * (1f / FadeTime);
            yield return null;
        }
        Destroy(gameObject);
    }
}