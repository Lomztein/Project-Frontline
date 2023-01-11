using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnEffect : MonoBehaviour
{
    public ParticleSystem Particle;
    public float ParticleLife;
    public float EmissionRateMultiplier;
    public GameObject InitialTarget;

    private void Start ()
    {
        if (InitialTarget)
        {
            ApplyTo(InitialTarget);
        }
    }

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded)
        {
            Particle.transform.SetParent(null, true);
            Particle.Stop();
            Destroy(Particle.gameObject, ParticleLife);
        }
    }

    public void ApplyTo(GameObject target)
    {
        transform.position = target.transform.position;
        Collider col = target.GetComponentInChildren<Collider>();
        if (col)
        {
            ParticleSystem.ShapeModule shape = Particle.shape;
            shape.scale = col.bounds.size;
            shape.position = col.bounds.center - target.transform.position;

            ParticleSystem.EmissionModule emission = Particle.emission;
            emission.rateOverTime = col.bounds.size.magnitude * EmissionRateMultiplier;
        }
    }
}
