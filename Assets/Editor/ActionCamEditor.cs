using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using Util;

[CustomEditor(typeof(ActionCam))]
public class ActionCamEditor : Editor
{
    private ActionCam Cam => (ActionCam)target;
    private Transform PrefabTransform
        => PrefabStageUtility.GetCurrentPrefabStage().scene.GetRootGameObjects()[0].transform;
    private Transform _selectedTransform;

    private void SetSelectedParent (Transform transform)
    {
        Cam.TransformPath = transform.GetPath();
        Cam.TransformPath = Cam.TransformPath.Substring(Cam.TransformPath.IndexOf('/') + 1);
    }

    private Transform GetSelectedParent ()
    {
        if (string.IsNullOrWhiteSpace(Cam.TransformPath))
            return null;
        return PrefabTransform.Find(Cam.TransformPath);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (PrefabStageUtility.GetCurrentPrefabStage() != null)
        {
            _selectedTransform = (Transform)EditorGUILayout.ObjectField(_selectedTransform, typeof(Transform), true);
            if (GUILayout.Button("Sync Identifier"))
            {
                Cam.UnitIdentifier = PrefabTransform.GetComponent<Unit>().Info.Identifier;
            }
            if (GUILayout.Button("Sync Transform"))
            {
                SyncToSceneCamera();
            }
            if (GUILayout.Button("Sync Parent"))
            {
                SetSelectedParent(_selectedTransform);
            }
        }
    }

    private void SyncToSceneCamera()
    {
        Vector3 worldPos = SceneView.lastActiveSceneView.camera.transform.position;
        Quaternion worldRot = SceneView.lastActiveSceneView.camera.transform.rotation;

        Cam.LocalPosition = GetSelectedParent().InverseTransformPoint(worldPos);
        Cam.LocalRotation = (Quaternion.Inverse(GetSelectedParent().rotation) * worldRot);
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public void OnSceneGUI(SceneView view)
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() != null)
        {
            Transform selectedParent = GetSelectedParent();
            if (selectedParent)
            {
                Vector3 worldPos = selectedParent.TransformPoint(Cam.LocalPosition);
                Quaternion worldRot = selectedParent.rotation * Cam.LocalRotation;
                worldPos = Handles.PositionHandle(worldPos, worldRot);
                worldRot = Handles.RotationHandle(worldRot, worldPos);

                Cam.LocalPosition = selectedParent.InverseTransformPoint(worldPos);
                Cam.LocalRotation = (Quaternion.Inverse(selectedParent.rotation) * worldRot);
            }
            else
            {
                Cam.LocalPosition = Handles.PositionHandle(Cam.LocalPosition, Cam.LocalRotation);
                Cam.LocalRotation = Handles.RotationHandle(Cam.LocalRotation, Cam.LocalPosition);
            }
            EditorUtility.SetDirty(Cam);
        }
    }
}
