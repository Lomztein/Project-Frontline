using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ActionCam", menuName = "ActionCam")]
public class ActionCam : ScriptableObject
{
    public string UnitIdentifier = string.Empty;
    public string TransformPath = string.Empty;
    public Vector3 LocalPosition;
    public Quaternion LocalRotation = Quaternion.identity;
}
