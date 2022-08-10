using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parachute : MonoBehaviour
{
    public float HeightThreshold;
    public float FallSpeed;

    public InfantryGarrison Garrison;

    private void FixedUpdate()
    {
        transform.position += FallSpeed * Time.fixedDeltaTime * Vector3.down;
        if (transform.position.y <= HeightThreshold)
        {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
            Garrison.EvacuateAll();
            Destroy(gameObject);
        }
    }
}
