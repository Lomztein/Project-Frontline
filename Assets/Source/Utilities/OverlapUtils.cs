using UnityEngine;
using System;
using Unity.XR.CoreUtils;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

public static class OverlapUtils
{
    public static OverlapGroup CreateFromCollidersIn(GameObject obj, Predicate<Collider> filter = null)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        List<OverlapShape> shapes = new List<OverlapShape>(colliders.Length);
        Transform root = obj.transform;
        foreach (Collider collider in colliders)
        {
            if (filter != null && !filter(collider))
                continue;

            Vector3 pos = root.InverseTransformPoint(collider.transform.position);
            Quaternion rot = Quaternion.Inverse(collider.transform.rotation) * root.rotation;
            if (collider is BoxCollider box)
            {
                shapes.Add(new OverlapBox(pos + box.center, rot, box.size / 2f));
            }
            if (collider is SphereCollider sphere)
            {
                shapes.Add(new OverlapSphere(pos + sphere.center, rot, sphere.radius));
            }
            if (collider is CapsuleCollider capsule)
            {
                shapes.Add(new OverlapCapsule(pos + capsule.center, rot, capsule.radius, capsule.height));
            }
        }
        return new OverlapGroup(Vector3.zero, Quaternion.identity, shapes.ToArray());
    }

    public abstract class OverlapShape
    {
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;

        public abstract Collider[] Overlap(Vector3 position, Quaternion rotation, LayerMask layer);

        public OverlapShape(Vector3 localPosition, Quaternion localRotation)
        {
            LocalPosition = localPosition;
            LocalRotation = localRotation;
        }
    }

    public class OverlapBox : OverlapShape
    {
        public Vector3 Size;

        public override Collider[] Overlap(Vector3 position, Quaternion rotation, LayerMask layer)
        {
            return Physics.OverlapBox(position + LocalPosition, Size, rotation * LocalRotation, layer);
        }

        public OverlapBox(Vector3 localPosition, Quaternion localRotation, Vector3 size) : base(localPosition, localRotation)
        {
            Size = size;
        }
    }

    public class OverlapSphere : OverlapShape
    {
        public float Radius;

        public override Collider[] Overlap(Vector3 position, Quaternion rotation, LayerMask layer)
        {
            return Physics.OverlapSphere(position + LocalPosition, Radius, layer);
        }

        public OverlapSphere(Vector3 localPosition, Quaternion localRotation, float radius) : base(localPosition, localRotation)
        {
            Radius = radius;
        }
    }

    public class OverlapCapsule : OverlapShape
    {
        public float Radius;
        public float Height;

        public override Collider[] Overlap(Vector3 position, Quaternion rotation, LayerMask layer)
        {
            return Physics.OverlapCapsule(position + LocalPosition + Vector3.down * Height / 2, position + LocalPosition + Vector3.up * Height / 2f, Radius, layer);
        }

        public OverlapCapsule(Vector3 localPosition, Quaternion localRotation, float radius, float height) : base(localPosition, localRotation)
        {
            Radius = radius;
            Height = height;
        }
    }

    public class OverlapGroup : OverlapShape
    {
        private OverlapShape[] _shapes;
        
        public OverlapGroup (Vector3 localPosition, Quaternion localRotation, params OverlapShape[] shapes) : base(localPosition, localRotation)
        {
            _shapes = shapes;
        }

        public override Collider[] Overlap(Vector3 position, Quaternion rotation, LayerMask layer)
        {
            return _shapes.SelectMany(x => x.Overlap(position + LocalPosition, rotation * LocalRotation, layer)).Distinct().ToArray();
        }
    }
}