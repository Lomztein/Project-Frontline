using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightHitscanProjectileRenderer : HitscanProjectileRenderer
{
    public LineRenderer Renderer;
    public float StartingWidth;

    public float ShrinkTime = 0.4f;

    public override void SetPositions(Vector3 start, Vector3 end)
    {
        Renderer.startWidth = StartingWidth;
        Renderer.endWidth = StartingWidth;
        Renderer.SetPosition(0, start);
        Renderer.SetPosition(1, end);
        StartCoroutine(ShrinkBeam(ShrinkTime));
    }

    private IEnumerator ShrinkBeam (float time)
    {
        int ticks = Mathf.RoundToInt(time / Time.fixedDeltaTime);
        for (int i = 0; i < ticks; i++)
        {
            float progress = i / (float)ticks;
            float width = (1 - progress) * StartingWidth;
            Renderer.startWidth = width;
            Renderer.endWidth = width;
            yield return new WaitForFixedUpdate();
        }
        Renderer.startWidth = 0f;
        Renderer.endWidth = 0f;
    }
}
