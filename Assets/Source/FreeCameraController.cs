#if ENABLE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM_PACKAGE
#define USE_INPUT_SYSTEM
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.Controls;
#endif

using UnityEngine;
using UnityEngine.UIElements;

namespace UnityTemplateProjects
{
    public class FreeCameraController : MonoBehaviour, ICameraController
    {
        class CameraState
        {
            public Vector3 position;
            public Quaternion rotation;

            public void SetFromTransform(Transform t)
            {
                position = t.position;
                rotation = t.rotation;
            }

            public void Translate(Vector3 translation)
            {
                Vector3 rotatedTranslation = rotation * translation;
                position += rotatedTranslation;
            }

            public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
            {
                rotation = Quaternion.Slerp(rotation, target.rotation, rotationLerpPct);
                position = Vector3.Lerp(position, target.position, positionLerpPct);
            }

            public void UpdateTransform(Transform t)
            {
                t.rotation = rotation;
                t.position = position;
            }
        }

        CameraState m_TargetCameraState = new CameraState();
        CameraState m_InterpolatingCameraState = new CameraState();

        [Header("Movement Settings")]
        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        public float boost = 3.5f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        public float positionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        public float mouseSensitivity = 3;

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        public float rotationLerpTime = 0.01f;

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
        public bool invertY = false;

        void OnEnable()
        {
            m_TargetCameraState.SetFromTransform(transform);
            m_InterpolatingCameraState.SetFromTransform(transform);
        }

        void Update()
        {
            if (Application.isFocused)
            {
                // Framerate-independent interpolation
                // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
                var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.unscaledDeltaTime);
                var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.unscaledDeltaTime);
                m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

                m_InterpolatingCameraState.UpdateTransform(transform);
            }
        }

        public void Pan(Vector2 movement)
        {
            movement *= Mathf.Pow(2.0f, boost);
            m_TargetCameraState.Translate(new Vector3(movement.x, 0f, movement.y));
        }

        public void Rotate(Vector2 rotation)
        {
            rotation *= mouseSensitivity;
            m_TargetCameraState.rotation *= Quaternion.Euler(-rotation.y, rotation.x, 0f);
            Vector3 euler = m_TargetCameraState.rotation.eulerAngles;
            m_TargetCameraState.rotation = Quaternion.Euler(euler.x, euler.y, 0f);
        }

        public void Zoom(float amount)
        {
            boost += amount * 0.05f;
        }

        public void LookAt(Vector3 position)
        {
            m_TargetCameraState.rotation = Quaternion.LookRotation(m_TargetCameraState.position - position);
        }

        public void TransitionFrom(Vector3 position, Quaternion rotation)
        {
            m_TargetCameraState.position = position;
            m_TargetCameraState.rotation = rotation;
        }
    }
}