﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Util
{
    public static class UnityUtils
    {
        public const float DEFAULT_FIXED_DELTA_TIME = 0.02f;
        public static float FixedDeltaTimeFactor => Time.fixedDeltaTime / DEFAULT_FIXED_DELTA_TIME;

        public static GameObject InstantiateMockGO (GameObject original)
        {
            // First create object and strip away all non-transform non-renderer components.
            GameObject model = UnityEngine.Object.Instantiate(original);

            var transforms = model.GetComponentsInChildren<Transform>(true);

            List<Component> nonVitals = model.GetComponentsInChildren<Component>().Where(x => !(x is Transform) && !(x is Renderer) && !(x is MeshFilter) && !(x is Rigidbody)).ToList();
            foreach (Component comp in nonVitals)
            {
                UnityEngine.Object.Destroy(comp); // Might not be neccesary, test sometime.
            }
            model.SetActive(true);

            return model;
        }

        public static IEnumerator WaitForFixedSeconds(float seconds)
        {
            int frames = Mathf.RoundToInt(seconds / Time.fixedDeltaTime);
            for (int i = 0; i < frames; i++)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        public static void Scale(this ParticleSystem system, float scale)
        {
            system.transform.localScale = Vector3.one * scale;
            var main = system.main;

            main.startSizeMultiplier = scale;
            main.startSpeedMultiplier = scale;

            var emission = system.emission;

            emission.rateOverTimeMultiplier = scale;
            emission.rateOverDistanceMultiplier = scale;
            var bursts = new ParticleSystem.Burst[emission.burstCount];

            emission.GetBursts(bursts);

            for (int i = 0; i < bursts.Length; i++)
            {
                bursts[i].maxCount *= (short)(scale);
                bursts[i].minCount *= (short)(scale);
            }
        }

        public static float ComputeSimpleDrag(float speed, float dragCoeffecient)
            => dragCoeffecient * (Mathf.Pow(speed, 2) / 2f);

        public static Vector3 ComputeSimpleDragForce (Vector3 velocity, float dragCoeffecient)
        {
            float magnitude = velocity.magnitude;
            float force = ComputeSimpleDrag(magnitude, dragCoeffecient);
            Vector3 perp = Vector3.Cross(velocity, Vector3.up);
            Vector3 refl = Vector3.Reflect(velocity, perp);
            return refl / magnitude * force * -1f;
        }

        public static string GetPath(this Transform transform)
        {
            Transform current = transform;
            string path = "";
            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }
            if (string.IsNullOrEmpty(path))
                return String.Empty;
            return path.Substring(0, path.Length - 1);
        }
    }
}
