using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldButtonMaterialAdderRemover : MonoBehaviour
{
    public WorldButton WorldButton;
    public Material IdleMaterial;
    public Material HoverMaterial;
    public Material ClickMaterial;

    private Material _currentBonusMaterial;
    private Renderer[] _renderers;

    private void Start()
    {
        WorldButton.PointerEnter.AddListener(OnPointerEnter);
        WorldButton.PointerExit.AddListener(OnPointerExit);
        WorldButton.PointerDown.AddListener(OnPointerDown);
        WorldButton.PointerUp.AddListener(OnPointerUp);
    }

    private void OnPointerEnter(WorldPointer arg0)
    {
        SetBonusMaterialToAll(_renderers, HoverMaterial);
    }

    private void OnPointerExit(WorldPointer arg0)
    {
        SetBonusMaterialToAll(_renderers, IdleMaterial);
    }

    private void OnPointerDown(WorldPointer arg0, int arg1)
    {
        SetBonusMaterialToAll(_renderers, ClickMaterial);
    }

    private void OnPointerUp(WorldPointer arg0, int arg1)
    {
        SetBonusMaterialToAll(_renderers, HoverMaterial);
    }

    private void SetBonusMaterialToAll(Renderer[] renderers, Material material)
    {
        foreach (var  renderer in renderers)
        {
            SetBonusMaterialToRenderer(renderer, material, _currentBonusMaterial);
        }
        _currentBonusMaterial = material;
    }

    private void SetBonusMaterialToRenderer(Renderer renderer, Material material, Material prev)
    {
        List<Material> newMaterials = new List<Material>(renderer.materials);
        if (prev != null)
        {
            newMaterials.RemoveAll(x => x.name.StartsWith(prev.name));
        }
        if (material != null)
        {
            newMaterials.Add(material);
        }
        renderer.SetMaterials(newMaterials);
    }

    public void Assign(Renderer[] renderers)
    {
        _renderers = renderers;
    }
}