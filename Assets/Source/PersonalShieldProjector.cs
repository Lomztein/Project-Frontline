using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalShieldProjector : MonoBehaviour
{
    public ShieldProjector Projector;

    public float ShieldSizeMultiplier;
    public float ShieldHealthFactor;
    public float TotalHealTime;

    private void Start()
    {
        // TODO: Look into offset centers, eg. the jet
        transform.rotation = transform.parent.rotation;

        Bounds bounds = new Bounds();
        Collider[] colliders = transform.root.GetComponentsInChildren<Collider>(false);
        foreach (Collider col in colliders)
        {
            Bounds b = new Bounds(col.bounds.center - col.transform.position, col.bounds.size);
            bounds.Encapsulate(b);
        }
        transform.position = bounds.center + transform.transform.position;

        Projector.ShieldSize = bounds.size.magnitude * ShieldSizeMultiplier;
        float unitHealth = transform.root.GetComponentInChildren<Health>().MaxHealth;

        Projector.GetComponentInChildren<Health>().MaxHealth = unitHealth * ShieldHealthFactor;
        Projector.HealRate = unitHealth / TotalHealTime;
        Projector.ForceResetSize();
    }
}
