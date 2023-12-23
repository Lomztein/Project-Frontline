using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSwarm : MonoBehaviour, IEnumerable<Boid>
{
    public List<Boid> Boids;
    public Health Health; // Total hack lol

    public IEnumerator<Boid> GetEnumerator()
    {
        return ((IEnumerable<Boid>)Boids).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Boids.GetEnumerator();
    }

    private void Awake()
    {
        foreach (Boid boid in this)
        {
            boid.transform.position += Random.insideUnitSphere;
        }
    }

    public void OnBoidDestroyed (Boid boid)
    {
        Boids.Remove(boid);
        if (Boids.Count <= 0)
        {
            Health.TakeDamage(new DamageInfo(Health.MaxHealth * 2f, DamageModifier.One, Vector3.forward, Vector3.forward, this, Health));
        }
    }

    public void AddBoid(Boid boid)
        => Boids.Add(boid);
}
