using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnStart : MonoBehaviour
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
