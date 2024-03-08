using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

public class EquipmentInfo : MonoBehaviour
{
    public string Name;
    public string Description;
    public Component MainComponent;
    [FormerlySerializedAs("Root")]
    public GameObject Model;
    [SerializeReference, SR]
    public WeaponAimBehaviour AimBehaviour;
    public Renderer[] EquipmentRenderers;
    public Transform ParentOverride;

    public Transform GetBoundsParent()
        => ParentOverride == null ? gameObject.transform : ParentOverride;
}
