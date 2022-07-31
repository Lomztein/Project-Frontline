using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCameraController : MonoBehaviour
{
    public ActionCamera[] Cameras;
    public ActionCamera CurrentCamera;
    public Transform CurrentParent;
    public float SwitchTime = 30f;

    public void Update()
    {
        if (CurrentParent == null)
        {
            FindActionCamera();
            if (CurrentParent)
            {
                CancelInvoke();
                Invoke(nameof(Switch), SwitchTime);
            }
        }
    }

    public void LateUpdate()
    {
        if (CurrentParent)
        {
            transform.position = CurrentParent.TransformPoint(CurrentCamera.LocalPosition);
            transform.rotation = CurrentParent.transform.rotation * Quaternion.Euler(CurrentCamera.LocalRotation);
        }
    }

    private void FindActionCamera()
    {
        var camera = Cameras[Random.Range(0, Cameras.Length)];
        var commanders = GameObject.FindGameObjectsWithTag("Commander");
        var commander = commanders[Random.Range(0, commanders.Length)].GetComponent<Commander>();

        Unit highest = null;
        float highestScore = float.MinValue;

        foreach (var unit in commander.AliveProduced)
        {
            if (unit != null && unit.Info.Identifier.StartsWith(camera.UnitIdentifier))
            {
                float score = unit.GetComponent<Health>().CurrentHealth * Random.Range(0.5f, 1f);
                if (score > highestScore)
                {
                    highestScore = score;
                    highest = unit;
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
        }
    }

    private void Switch ()
    {
        CurrentParent = null;
    }

    [System.Serializable]
    public class ActionCamera
    {
        public string UnitIdentifier;
        public string TransformPath;
        public Vector3 LocalPosition;
        public Vector3 LocalRotation;
    }
}
