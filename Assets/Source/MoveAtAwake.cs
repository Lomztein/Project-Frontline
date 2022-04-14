using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAtAwake : MonoBehaviour
{
    public Vector3 Movement;
    public bool Transformed;

    private void Awake()
    {
        if (Transformed)
        {
            transform.Translate(Movement);
        }
        else
        {
            transform.position += Movement;
        }
    }
}
