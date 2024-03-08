using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderMaterialApplier : MonoBehaviour
{
    [SerializeField] private Renderer[] _renderersToApplyTo;

    public void ApplyMaterial(Material mat)
    {
        foreach (Renderer renderer in _renderersToApplyTo)
        {
            if (renderer)
            {
                renderer.material = mat;
            }
            else
            {
                Debug.LogWarning("Renderer missing in material applier.");
            }
        }
    }
}
