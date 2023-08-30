﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public static T FindBest<T>(IEnumerable<T> options, Func<T, float> evaluator, Predicate<T> filter = null)
        {
            float high = float.MinValue;
            T best = default;
            foreach (T option in options)
            {
                if (filter != null)
                {
                    if (!filter(option))
                    {
                        continue;
                    }
                }
                float score = evaluator(option);
                if (score > high)
                {
                    high = score;
                    best = option;
                }
            }
            return best;
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

        // I stole this from the internet because I was too lazy to write it myself lol
        public static Transform FindRecursive(this Transform self, Predicate<Transform> selector)
        {
            foreach (Transform child in self)
            {
                if (selector(child))
                {
                    return child;
                }
                var finding = child.FindRecursive(selector);

                if (finding != null)
                {
                    return finding;
                }
            }

            return null;
        }

        /// <summary>
        /// Computes the given objects world-space visual bounds based on the objects mesh renderers.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Bounds ComputeObjectRendererBounds(GameObject obj)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>(false).Where(x => x is not ParticleSystemRenderer);
            Bounds bounds = renderers.First().bounds;
            foreach (var renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }
            return bounds;
        }

        /// <summary>
        /// Computes the given objects world-space collision bounds based on the objects colliders. Only works on active objects.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Bounds ComputeObjectColliderBounds(GameObject obj)
        {
            Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                if (collider.enabled)
                {
                    bounds.Encapsulate(collider.bounds); // That was almost dissapointingly easy.
                }
            }
            return bounds;
        }

        public static Vector3 RandomPointInside(this Bounds bounds)
        {
            Vector3 extents = bounds.extents;
            Vector3 point = new Vector3(
                UnityEngine.Random.Range(-extents.x, extents.x),
                UnityEngine.Random.Range(-extents.y, extents.y),
                UnityEngine.Random.Range(-extents.z, extents.z)
                );
            return point + bounds.center;
        }

        /// <summary>
        /// Computes the minimal bounding box based on object mesh verticies, and the rotation of the object.
        /// Probably fairly computationally expensive, avoid calling too much.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Bounds ComputeMinimallyBoundingBox(GameObject obj)
        {
            Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
            var filters = obj.GetComponentsInChildren<MeshFilter>();
            var skinnedRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var filter in filters)
            {
                var mesh = filter.sharedMesh;
                if (mesh != null)
                {
                    foreach (var vert in mesh.vertices)
                    {
                        Vector3 worldPos = filter.transform.TransformPoint(vert);
                        bounds.Encapsulate(worldPos);
                    }
                }
            }

            foreach (var renderer in skinnedRenderers)
            {
                Mesh mesh = renderer.sharedMesh;
                if (mesh != null)
                {
                    foreach (var vert in mesh.vertices)
                    {
                        Vector3 worldPos = renderer.transform.TransformPoint(vert);
                        bounds.Encapsulate(worldPos);
                    }
                }
            }

            return bounds;
        }

        public static Vector3 Flat(this Vector3 vec)
            => new Vector3(vec.x, 0f, vec.z);
    }
}
