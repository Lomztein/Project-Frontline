using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UnitCam", menuName = "UnitCam")]
public class UnitCam : ScriptableObject
{
    public string UnitIdentifier = string.Empty;
    public string TransformPath = string.Empty;
    public Vector3 LocalPosition;
    public Quaternion LocalRotation = Quaternion.identity;
}
