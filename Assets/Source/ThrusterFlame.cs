using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterFlame : MonoBehaviour
{
    public float BaseLengthScale;
    public float Variance;

    private Vector3 _baseScale;

    private void Awake()
    {
        _baseScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        transform.localScale = _baseScale + Vector3.forward * (BaseLengthScale + Random.Range(-Variance, Variance));
    }
}
