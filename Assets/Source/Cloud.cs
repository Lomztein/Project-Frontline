using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public Vector3 Movement;
    public ParticleSystem ParticleSystem;
    public float ParticleLife;

    public void End(float time)
    {
        Invoke(nameof(InternalEnd), time);
    }

    private void InternalEnd ()
    {
        ParticleSystem.Stop();
        Destroy(gameObject, ParticleLife);
    }

    private void Update()
    {
        transform.position += Movement * Time.deltaTime;
    }
}
