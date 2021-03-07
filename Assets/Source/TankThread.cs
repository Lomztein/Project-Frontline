using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankThread : MonoBehaviour
{
    public float ThreadLength;
    public Material ThreadMaterial;

    void Update()
    {
        
    }

    [System.Serializable]
    public class ThreadWheel
    {
        public float Circumference;
        public Transform WheelPivot;
    }
}
