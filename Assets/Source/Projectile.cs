using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed;
    public float Damage;
    private Vector3 _velocity;

    public float Life;
    public GameObject HitParticle;
    public float HitParticleLife;

    public void Fire (Vector3 direction)
    {
        Destroy(gameObject, Life);
        transform.LookAt(transform.position + direction);
        _velocity = Speed * direction;
    }
    private void FixedUpdate()
    {
        transform.position += _velocity * Time.fixedDeltaTime;
        if (Physics.Raycast(transform.position, transform.forward * Speed * Time.fixedDeltaTime, out RaycastHit hit, Speed * Time.fixedDeltaTime))
        {
            Destroy(gameObject);
            hit.collider.transform.root.SendMessage("TakeDamage", Damage, SendMessageOptions.DontRequireReceiver);
            Destroy (Instantiate(HitParticle, hit.point, Quaternion.LookRotation(hit.normal)), HitParticleLife);
        }
    }
}
