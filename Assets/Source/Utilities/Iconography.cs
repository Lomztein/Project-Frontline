using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Util;

namespace Util
{
    public class Iconography : MonoBehaviour
    {
        private static Iconography _instance;

        private const float DIST_FROM_WORLD = 4098f;

        private static Camera Camera => _instance.RenderCamera;
        public const int DEFAULT_RENDER_SIZE = 256;

        public Camera RenderCamera;
        public Vector3 ModelRotation;

        private void Awake()
        {
            _instance = this;
            gameObject.SetActive(false);
        }

        private static Vector3 GetPosition()
        {
            Quaternion rot = Quaternion.Euler(0f, 0f, 0f);
            return rot * (Vector3.one * DIST_FROM_WORLD);
        }

        public static GameObject InstantiateModel(GameObject source)
        {
            return UnityUtils.InstantiateMockGO(source);
        }

        public static Sprite GenerateSprite(GameObject obj, Quaternion objRotation, int renderSize)
        {
            Texture2D tex = GenerateIcon(obj, objRotation, renderSize);
            Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, renderSize, renderSize), Vector2.one / 2f);
            return sprite;
        }

        public static Vector2 ComputeCameraSize(Bounds bounds, Camera camera)
        {
            Matrix4x4 mat = Matrix4x4.TRS(bounds.center, camera.transform.rotation, Vector3.one).inverse;
            bounds.extents = mat.MultiplyPoint(bounds.extents);
            Debug.Log(bounds);
            Vector3 vec = mat.MultiplyPoint(bounds.extents);

            return new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z)); ;// new Vector3(xx, yy, zz) / 2f;
        }

        public static Sprite GenerateSprite(GameObject go) => GenerateSprite(go, Quaternion.Euler(_instance.ModelRotation), DEFAULT_RENDER_SIZE);

        public static Texture2D GenerateIcon(GameObject obj, Quaternion objRotation, int renderSize)
        {
            _instance.gameObject.SetActive(true);
            _instance.transform.position = GetPosition();

            Camera.enabled = true;
            Camera.aspect = 1f;

            GameObject model = InstantiateModel(obj);

            model.transform.position = _instance.transform.position;
            model.transform.rotation = objRotation;
            model.transform.localScale = Vector3.one;
            model.SetActive(true);

            Bounds bounds = UnityUtils.ComputeMinimallyBoundingBox(model);
            RenderTexture renderTexture = RenderTexture.GetTemporary(renderSize, renderSize, 24);

            Vector3 localCenter = _instance.transform.InverseTransformPoint(bounds.center);

            model.transform.position += -localCenter;
            //Camera.transform.LookAt(_instance.transform.position);

            RenderTexture.active = renderTexture;
            GL.Clear(true, true, Color.clear);

            Camera.targetTexture = renderTexture;
            float distance = bounds.size.magnitude;
            Camera.transform.position = _instance.transform.position + Vector3.back * distance;

            //Vector3 size = ComputeCameraSize(bounds, Camera);
            float camSize = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);

            Camera.orthographicSize = camSize;

            Camera.Render();

            Texture2D texture = new Texture2D(renderSize, renderSize, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0f, 0f, renderSize, renderSize), 0, 0);
            texture.Apply();

            RenderTexture.active = null;

            RenderTexture.ReleaseTemporary(renderTexture);
            Camera.targetTexture = null;

            Camera.enabled = false;
            _instance.gameObject.SetActive(false);

            model.transform.position += Vector3.right * 9999999f;
            model.SetActive(false);
            DestroyImmediate(model);
            return texture;
        }

        public static Texture2D GenerateIcon(GameObject go) => GenerateIcon(go, Quaternion.Euler(_instance.ModelRotation), DEFAULT_RENDER_SIZE);

        public static Bounds GetObjectBounds(GameObject obj)
        {
            Vector3 prevPos = obj.transform.position;
            Quaternion prevRot = obj.transform.rotation;

            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;

            Bounds bounds = new Bounds();
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            obj.transform.position = prevPos;
            obj.transform.rotation = prevRot;

            return bounds;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(transform.position, 0.25f);
            Gizmos.DrawWireSphere(transform.position + Vector3.back * 10f, 0.25f);
        }
    }
}