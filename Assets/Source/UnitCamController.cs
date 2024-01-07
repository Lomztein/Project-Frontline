using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;
using static UnityEngine.UI.CanvasScaler;

public class UnitCamController : MonoBehaviour
{
    public UnitCam[] Cameras;
    public UnitCam CurrentCamera;
    public Transform CurrentParent;
    public Health CurrentHealth;
    public Vector2 SwitchTime = new Vector2(20f, 40f);

    public float RotLerpTime;

    public float FindDelay = 2f;
    public float FreeDelay = 1f;

    public float FallbackHeight;
    public float FallbackLookHeight;
    private List<Transform> _fallbacks;

    public GameObjectFilter UnitFilter;
    public float FrontlineDistanceThreshold = 100;

    public enum EvaluatorFunction { Constant, Power, DPS, Health, Tier, Cost }
    public EvaluatorFunction UnitEvaluator;
    public Dictionary<EvaluatorFunction, Func<Unit, float>> _evaluatorFunctions = new Dictionary<EvaluatorFunction, Func<Unit, float>>()
    {
        { EvaluatorFunction.Constant, (x) => 1f },
        { EvaluatorFunction.Power, (x) => x.Health.MaxHealth * x.GetWeapons().Sum(y => y.GetDPS()) },
        { EvaluatorFunction.Cost, (x) => x.BaseCost },
        { EvaluatorFunction.Health, (x) => x.Health.MaxHealth },
        { EvaluatorFunction.DPS, (x) =>  x.GetWeapons().Sum(y => y.GetDPS()) },
        { EvaluatorFunction.Tier, (x) => (int)x.Info.UnitTier }
    };

    private void Awake()
    {
        GenerateFallbacks();
        Cameras = Resources.LoadAll<UnitCam>("UnitCam");
    }

    private void OnEnable()
    {
        FindActionCamera();
    }

    public void Update()
    {
        if (CurrentParent == null && !IsInvoking())
        {
            Invoke(nameof(FindActionCamera), FindDelay);
        }
    }

    private void LateUpdate()
    {
        if (CurrentParent && CurrentCamera != null)
        {
            transform.position = CurrentParent.TransformPoint(CurrentCamera.LocalPosition);
            transform.rotation = Quaternion.Lerp(transform.rotation, CurrentParent.transform.rotation * CurrentCamera.LocalRotation, RotLerpTime * Time.deltaTime);
        }
    }

    private void GenerateFallbacks ()
    {
        _fallbacks = new List<Transform>();
        var verts = MatchSettings.Current.MapInfo.GetPerimeterPolygon().ToArray();
        foreach (var vert in verts)
        {
            var pos = new Vector3(vert.z / 2f, FallbackHeight, vert.x / 2f);
            GameObject newFallback = new GameObject($"UnitCam Fallback ({pos})");
            newFallback.transform.position = pos;
            newFallback.transform.LookAt(Vector3.up * FallbackLookHeight);
            _fallbacks.Add(newFallback.transform);
        }

        for (int i = 0; i < verts.Length; i++)
        {
            var v1 = verts[i] * 1.5f;
            var v2 = verts[(i + 1) % verts.Length] * 1.5f;

            var pos = Vector3.Lerp(v1, v2, 0.5f);
            pos = new Vector3(pos.z / 2f, FallbackHeight, pos.x / 2f);

            GameObject newFallback = new GameObject($"UnitCam Fallback ({pos})");
            newFallback.transform.position = pos;
            newFallback.transform.LookAt(Vector3.up * FallbackLookHeight);
            _fallbacks.Add(newFallback.transform);
        }
    }

    private void FindActionCamera()
    {
        UnitCam camera = null;
        var commanders = GameObject.FindGameObjectsWithTag("Commander").Where(x => x.GetComponent<Commander>().AliveAll.Count() > 0).ToArray();
        if (commanders.Length > 0)
        {
            var commander = commanders[UnityEngine.Random.Range(0, commanders.Length)].GetComponent<Commander>();

            Unit highest = null;
            float highestScore = float.MinValue;
            var units = commander.AliveAll.Where(x => Filter(x.gameObject, commander)).Shuffle();

            foreach (var unit in units)
            {
                if (unit.CompareTag("StructureUnit") && !unit.IsEngaged)
                    continue;

                var unitCamera = GetRandomCamera(unit);
                if (unit != null && unitCamera != null)
                {
                    float score = _evaluatorFunctions[UnitEvaluator](unit) * UnityEngine.Random.Range(0f, 1f);
                    if (score > highestScore)
                    {
                        highestScore = score;
                        highest = unit;
                        camera = unitCamera;
                    }
                }
            }

            if (highest)
            {
                CurrentCamera = camera;
                if (string.IsNullOrEmpty(camera.TransformPath))
                {
                    CurrentParent = highest.transform;
                }
                else
                {
                    CurrentParent = highest.transform.Find(camera.TransformPath);
                }
                if (CurrentHealth)
                {
                    CurrentHealth.OnDeath -= ActionCameraController_OnDeath;
                }
                CurrentHealth = highest.GetComponent<Health>();
                CurrentHealth.OnDeath += ActionCameraController_OnDeath;
            }
            else
            {
                Fallback();
            }
        }
        else
        {
            Fallback();
        }

        CancelInvoke();
        Invoke(nameof(Switch), UnityEngine.Random.Range(SwitchTime.x, SwitchTime.y));
    }

    private void Fallback ()
    {
        CurrentParent = _fallbacks[UnityEngine.Random.Range(0, _fallbacks.Count)];
        CurrentCamera = ScriptableObject.CreateInstance<UnitCam>();
    }

    private void ActionCameraController_OnDeath(Health obj)
    {
        obj.OnDeath -= ActionCameraController_OnDeath;
        if (CurrentHealth == obj)
        {
            CancelInvoke();
            Invoke(nameof(FindActionCamera), FindDelay);
        }
    }

    public void Free ()
    {
        CurrentCamera = null;
    }

    private UnitCam GetRandomCamera(Unit target)
    {
        var cameras = Cameras.Where(x => Matches(target, x)).ToArray();
        if (cameras.Length > 0)
        {
            return cameras[UnityEngine.Random.Range(0, cameras.Length)];
        }
        return null;
    }

    private bool Matches(Unit unit, UnitCam cam)
    {
        string id = cam.UnitIdentifier;
        if (!string.IsNullOrEmpty(cam.TransformPath))
        {
            // Check if cam parent exists, and return false if not. This should allow us to attach cameras to things that aren't always there such as upgrades.
            if (unit.transform.root.Find(cam.TransformPath) == null)
                return false;
        }
        return unit.Info.Identifier.StartsWith(id);
    }

    private bool Filter(GameObject unit, Commander com)
    {
        if (UnitFilter != null && !UnitFilter.Check(unit))
            return false;
        if (Vector3.Distance(unit.transform.position, com.Frontline.Position) > FrontlineDistanceThreshold)
            return false;
        if (unit.transform.root.CompareTag("StructureUnit"))
            return false;
        if (unit.transform.lossyScale.magnitude < 0.5f)
            return false;
        return true;
    }

    private void Switch ()
    {
        FindActionCamera();
    }
}
