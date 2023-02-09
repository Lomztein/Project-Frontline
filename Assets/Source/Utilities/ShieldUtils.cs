using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public static class ShieldUtils
    {
        public static Transform GetShieldInObj(GameObject obj)
        {
            return obj.transform.FindRecursive(x => x.CompareTag("Shield"));
        }

        public static float ComputeShieldRadius(Transform shield)
        {
            ShieldProjector projector = shield.root.GetComponentInChildren<ShieldProjector>(); // Yes this is trash idc, for some reason GetComponentInParent doesn't work.
            if (projector == null)
            {
                SphereCollider col = shield.GetComponent<SphereCollider>();
                float size = col.radius * shield.localScale.x; // Assuming all elements of size is equal.
                return size;
            }
            else
            {
                return projector.ShieldSize / 2f;
            }
        }
    }
}