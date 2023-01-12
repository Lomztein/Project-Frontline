using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionCameraController : MonoBehaviour
{
    public ActionCam[] Cameras;
    public ActionCam CurrentCamera;
    public Transform CurrentParent;
    public Health CurrentHealth;
    public Vector2 SwitchTime = new Vector2(20f, 40f);

    public float RotLerpTime;

    public float FindDelay = 2f;
    public float FreeDelay = 1f;

    public float FallbackHeight;
    public float FallbackLookHeight;
    private List<Transform> _fallbacks;

    private void Awake()
    {
        GenerateFallbacks();
        Cameras = Resources.LoadAll<ActionCam>("ActionCam");
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
            GameObject newFallback = new GameObject($"ActionCam Fallback ({pos})");
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

            GameObject newFallback = new GameObject($"ActionCam Fallback ({pos})");
            newFallback.transform.position = pos;
            newFallback.transform.LookAt(Vector3.up * FallbackLookHeight);
            _fallbacks.Add(newFallback.transform);
        }
    }

    private void FindActionCamera()
    {
        ActionCam camera = null;
        var commanders = GameObject.FindGameObjectsWithTag("Commander").Where(x => x.GetComponent<Commander>().AliveAll.Count() > 0).ToArray();
        if (commanders.Length > 0)
        {
            var commander = commanders[Random.Range(0, commanders.Length)].GetComponent<Commander>();

            Unit highest = null;
            float highestScore = float.MinValue;

            foreach (var unit in commander.AliveAll)
            {
                if (unit.CompareTag("StructureUnit") && !unit.IsEngaged)
                    continue;

                var unitCamera = GetRandomCamera(unit);
                if (unit != null && unitCamera != null)
                {
                    float score = (unit.GetWeapons().Sum(x => x.GetDPSOrOverride()) + unit.GetComponent<Health>().CurrentHealth) * Random.Range(0f, 1f);
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
        Invoke(nameof(Switch), Random.Range(SwitchTime.x, SwitchTime.y));
    }

    private void Fallback ()
    {
        CurrentParent = _fallbacks[Random.Range(0, _fallbacks.Count)];
        CurrentCamera = ScriptableObject.CreateInstance<ActionCam>();
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

    private ActionCam GetRandomCamera(Unit target)
    {
        var cameras = Cameras.Where(x => Matches(target, x)).ToArray();
        if (cameras.Length > 0)
        {
            return cameras[Random.Range(0, cameras.Length)];
        }
        return null;
    }

    private bool Matches(Unit unit, ActionCam cam)
    {
        string id = cam.UnitIdentifier;
        return unit.Info.Identifier.StartsWith(id);
    }

    private void Switch ()
    {
        FindActionCamera();
    }
}
