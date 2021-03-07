using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sunglasses : MonoBehaviour
{
    public Transform NoseFulcrum;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(NoseFulcrum, Vector3.up);
    }
}
