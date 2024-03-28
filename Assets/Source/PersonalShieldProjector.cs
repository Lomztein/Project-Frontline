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
        StartCoroutine(DelayedGenerate());
    }

    private IEnumerator DelayedGenerate ()
    {
        yield return new WaitForFixedUpdate();

        // TODO: Look into offset centers, eg. the jet
        transform.rotation = transform.parent.rotation;
        transform.position = transform.parent.position;

        Bounds bounds = new Bounds();
        Collider[] colliders = transform.parent.GetComponentsInChildren<Collider>(false);
        foreach (Collider col in colliders)
        {
            Bounds b = new Bounds(col.bounds.center - col.transform.position, col.bounds.size);
            bounds.Encapsulate(b);
        }
        transform.position = bounds.center + transform.parent.position;

        float shieldSize = bounds.size.magnitude * ShieldSizeMultiplier;
        float shieldHealth = transform.parent.GetComponentInChildren<Health>().MaxHealth * ShieldHealthFactor;
        if (transform.parent.TryGetComponent(out PersonalShieldProjectorStatsOverride sizeOverride)) {
            if (sizeOverride.DoOverrideSize)
                shieldSize = sizeOverride.OverrideSize;
            if (sizeOverride.DoOverrideHealth)
                shieldHealth = sizeOverride.OverrideHealth;
        }
        Projector.ShieldSize = shieldSize;

        Projector.GetComponentInChildren<Health>().MaxHealth = shieldHealth * ShieldHealthFactor;
        Projector.HealRate = shieldHealth / TotalHealTime;
        Projector.ForceResetSize();
    }

    private void OnDrawGizmosSelected()
    {
        Bounds bounds = new Bounds();
        Collider[] colliders = transform.parent.GetComponentsInChildren<Collider>(false);
        foreach (Collider col in colliders)
        {
            Bounds b = new Bounds(col.bounds.center - col.transform.position, col.bounds.size);
            bounds.Encapsulate(b);
        }

        Gizmos.DrawWireCube(bounds.center, bounds.size);
        Gizmos.DrawWireSphere(bounds.center, bounds.size.magnitude * ShieldSizeMultiplier / 2f);
    }
}
