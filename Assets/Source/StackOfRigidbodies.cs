using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StackOfRigidbodies : MonoBehaviour
{
    public Rigidbody[] Rigidbodies;
    private Vector3[] _startingPositions;
    private Quaternion[] _startingRotations;
    [SerializeField] private Vector3 _localCenter;
    public Vector3 Center => transform.TransformPoint(_localCenter);

    public void Start()
    {
        _startingPositions = Rigidbodies.Select(x => x.transform.position).ToArray();
        _startingRotations = Rigidbodies.Select(x => x.transform.rotation).ToArray();
    }

    public void Reset()
    {
        for (int i = 0; i < Rigidbodies.Length; i++)
        {
            Rigidbodies[i].transform.position = _startingPositions[i];
            Rigidbodies[i].transform.rotation = _startingRotations[i];
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(Center, 0.5f);
    }
}
