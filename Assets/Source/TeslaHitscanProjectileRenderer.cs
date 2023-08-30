using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaHitscanProjectileRenderer : HitscanProjectileRenderer
{
    public LineRenderer Renderer;
    public Projectile Projectile;

    public int SegmentLength;
    public float ArcSpeed;
    public float DesturbanceFactor;
    public float UpwardsForce;
    public float ShrinkLerp;
    public float Width;

    public float MaxResetTime;
    public float MinResetTime;

    private int _arcDir;
    private Vector3 _start;
    private Vector3 _end;
    private int _sections;

    private float _spawnTime;
    public AnimationCurve WidthByLife;

    public override void SetPositions(Vector3 start, Vector3 end)
    {
        _arcDir = Random.Range(0, 2) == 1 ? -1 : 1;
        _sections = Mathf.FloorToInt(Vector3.Distance(start, end) / SegmentLength);
        _start = start;
        _end = end;
        Renderer.positionCount = _sections + 1;
        _spawnTime = Time.time;
        ResetLine();
        CycleReset();
    }

    private void FixedUpdate()
    {
        UpdateLines();
    }

    private void ResetLine ()
    {
        Vector3 between = (_end - _start) / _sections;

        for (int i = 0; i < _sections + 1; i++)
        {
            Vector3 newPos = _start + between * i;
            Vector3 r = Random.insideUnitSphere * DesturbanceFactor;
            Vector3 newRandom = new Vector3(r.x, r.y, r.z);
            if (i == 0)
            {
                newPos = _start;
                newRandom = Vector3.zero;
            }
            if (i == _sections)
            {
                newPos = _end;
                newRandom = Vector3.zero;
            }
            Renderer.SetPosition(i, newPos + newRandom);
        }

        float mult = CalcWidthMult();
        Renderer.endWidth = Width * mult;
        Renderer.startWidth = Width * mult;
    }
    
    private float CalcWidthMult ()
    {
        float timeFactor = Mathf.InverseLerp(_spawnTime, _spawnTime + Projectile.Life, Time.time);
        return WidthByLife.Evaluate(timeFactor);
    }

    private void CycleReset ()
    {
        CancelInvoke();
        Invoke(nameof(CycleReset), Random.Range(MinResetTime, MaxResetTime));
        ResetLine();
    }

    private void UpdateLines()
    {
        for (int i = 0; i < _sections; i++)
        {
            Vector3 rot = Quaternion.Euler(90f + Random.Range(20f, -20f), 90f + Random.Range(20f, -20f), 0f) * Projectile.Velocity.normalized;
            Vector3 nextPos = Renderer.GetPosition(i) + ((_end - _start).magnitude / 20f) * ArcSpeed * DesturbanceFactor * Mathf.Sin((float)i / _sections * Mathf.PI) * Random.Range(-2f * _arcDir, 4f * _arcDir) * Time.fixedDeltaTime * rot;
            nextPos.y = Mathf.Max(nextPos.y, Width / 2f);
            Renderer.SetPosition(i, nextPos);
        }

        float mult = CalcWidthMult();
        Renderer.endWidth = Mathf.Lerp(Renderer.endWidth, 0f, ShrinkLerp * Time.fixedDeltaTime) * mult;
        Renderer.startWidth = Mathf.Lerp(Renderer.endWidth, 0f, ShrinkLerp * Time.fixedDeltaTime) * mult;
    }
}
