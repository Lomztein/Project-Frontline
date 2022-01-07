using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MobileBody
{
    public BoidSwarm Swarm;
    public Transform Center;

    private Vector3 _velocity;
    public float SeperationDistance = 1;
    public float SeperationStrength = 1;
    public float CohesionStrength = 1;
    public float AccelerationStrength = 1;

    public float Drag = 0.95f;

    public override float CurrentSpeed { get; protected set; }

    // Update is called once per frame
    void FixedUpdate()
    {
        _velocity += Seperation() * Time.fixedDeltaTime + Cohesion() * Time.fixedDeltaTime + Acceleration() * Time.fixedDeltaTime;
        _velocity *= Drag;

        if (_velocity.sqrMagnitude > MaxSpeed * MaxSpeed)
        {
            _velocity = _velocity.normalized * MaxSpeed;
        }

        transform.LookAt(transform.position + _velocity);
        transform.position += _velocity * Time.fixedDeltaTime;

    }

    // Based on Boids, but no alignment and cohesion uses Center transform as target.
    private Vector3 Seperation ()
    {
        Vector3 result = new Vector3();
        foreach (Boid boid in Swarm)
        {
            Vector3 rel = transform.position - boid.transform.position;
            Vector3 dir = rel.normalized;
            float strength = Mathf.Clamp(SeperationDistance - rel.magnitude, 0f, SeperationDistance);
            result += SeperationStrength * strength * dir;
        }
        return result;
    }
    private Vector3 Cohesion ()
    {
        return (Center.position - transform.position).normalized * CohesionStrength;
    }

    private Vector3 Acceleration()
    {
        return transform.forward * AccelerationStrength;
    }

    private void OnDestroy()
    {
        if (Swarm)
        {
            Swarm.OnBoidDestroyed(this);
        }
    }
}
