using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

public class WeaponInfo : MonoBehaviour
{
    public string Name;
    public string Description;
    [FormerlySerializedAs("Root")]
    public GameObject Model;
    [SerializeReference, SR]
    public WeaponAimBehaviour AimBehaviour;
}
