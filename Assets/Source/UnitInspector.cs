using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UnitInspector : MonoBehaviour
{
    public ObjectDisplayScene DisplayScene;
    public Unit CurrentUnitPrefab;
    public GameObject EquipmentButtonPrefab;

    public TMPro.TMP_Text BasicInfoText;
    public TMPro.TMP_Text EquipmentInfoText;

    public RenderTexture RenderTexture;
    public UICameraViewport CameraViewport;

    private Unit _inspectingUnit;

    public bool UnitFiring;
    public bool UnitMoving;

    public WorldPointer WorldPointer;

    private void Start()
    {
        StartCoroutine(DelayedInit());
        Inspect(CurrentUnitPrefab.gameObject);
    }

    public void SetUnitFiring(bool firing)
        => UnitFiring = firing;
    public void SetUnitMoving(bool moving)
        => UnitMoving = moving;

    private IEnumerator DelayedInit()
    {
        yield return new WaitForFixedUpdate();
        RenderTexture = Instantiate(RenderTexture);
        CameraViewport.SetRenderTexture(RenderTexture);
        DisplayScene.OrbitCamera.GetComponent<Camera>().targetTexture = RenderTexture;
    }

    public GameObject Inspect(GameObject unitPrefab)
    {
        _inspectingUnit = DisplayScene.Display(unitPrefab).GetComponent<Unit>();
        Unit unit = unitPrefab.GetComponent<Unit>();
        BasicInfoText.text = GenerateBasicInfoString(unit);
        CreateEquipmentButtons(_inspectingUnit);

        foreach (IWeapon weapon in unit.GetWeapons())
        {
            weapon.SetHitLayerMask(LayerMask.GetMask("Terrain"));
        }

        return _inspectingUnit.gameObject;
    }

    public void CreateEquipmentButtons(Unit unit)
    {
        EquipmentInfo[] infos = unit.GetComponentsInChildren<EquipmentInfo>();
        foreach (var info in infos)
        {
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(info.EquipmentRenderers.Select(x => CreateMeshCombineInstanceFrom(x, info.GetBoundsParent())).ToArray());

            GameObject newButton = Instantiate(EquipmentButtonPrefab);
            newButton.transform.SetParent(info.GetBoundsParent());
            newButton.transform.localPosition = Vector3.zero;
            newButton.transform.localScale = Vector3.one;
            newButton.transform.localRotation = Quaternion.identity;
            newButton.GetComponent<MeshCollider>().sharedMesh = mesh;
            newButton.GetComponent<WorldButton>().PointerDown.AddListener((p, b) => InspectEquipment(info));
            newButton.GetComponent<WorldButtonMaterialAdderRemover>().Assign(info.EquipmentRenderers);
        }
    }

    private CombineInstance CreateMeshCombineInstanceFrom(Renderer renderer, Transform parent)
    {
        Vector3 pos = parent.InverseTransformPoint(renderer.transform.position);
        Mesh mesh = null;
        if (renderer.TryGetComponent(out MeshFilter filter))
        {
            mesh = filter.sharedMesh;
        }
        if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
        {
            mesh = skinnedMeshRenderer.sharedMesh;
        }

        CombineInstance instance = new()
        {
            mesh = mesh,
            transform = parent.worldToLocalMatrix * renderer.transform.localToWorldMatrix
        };
        return instance;
    }

    private void PadAlongNormals(Mesh mesh, float amount)
    {
        Vector3[] verts = mesh.vertices;
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            verts[i] += mesh.normals[i] * amount;
        }
        mesh.vertices = verts;
        mesh.RecalculateBounds();
        mesh.MarkModified();
    }

    private void InspectEquipment(EquipmentInfo info)
    {
        EquipmentInfoText.text = GetEquipmentInspectString(info);
    }

    private string GenerateBasicInfoString(Unit unit)
    {
        return $"<b>{unit.Info.Name}</b> - {unit.BaseCost}$" +
            $"\n{unit.Info.UnitRole} {unit.Info.UnitType}" +
            $"\n\n{unit.Info.Description}";
    }

    private void FixedUpdate()
    {
        if (_inspectingUnit != null)
        {
            if (UnitFiring)
            {
                foreach (var weapon in _inspectingUnit.GetWeapons())
                {
                    weapon.TryFire(new PositionTarget(_inspectingUnit.transform.forward * 1000));
                }
            }

            if (UnitMoving)
            {
                IControllable controllable = _inspectingUnit.GetComponent<IControllable>();
                controllable.Accelerate(1f);
            }
        }

        Ray ray = CameraViewport.ScreenPointToRay(Mouse.current.position.ReadValue());
        WorldPointer.Point(ray);
        WorldPointer.UpdateButtonState(new bool[] {
            Mouse.current.leftButton.ReadValue() > 0,
            Mouse.current.rightButton.ReadValue() > 0
            });
    }

    private string GetEquipmentInspectString(EquipmentInfo info)
    {
        StringBuilder builder = new();
        // Basic info.
        builder.AppendLine($"<b>{info.Name}</b>");
        builder.AppendLine(info.Description);
        builder.AppendLine(string.Empty);

        // Detail info.
        Component component = info.MainComponent;
        if (component is IWeapon weapon)
        {
            builder.AppendLine($"Damage: {weapon.Damage}");
            builder.AppendLine($"Firerate: {weapon.Firerate}");
            builder.AppendLine($"Damage Type: {weapon.Modifier.Name}");
            builder.AppendLine($"DPS: {weapon.GetDPSOrOverride()}");
        }

        if (component is Health health)
        {
            builder.AppendLine($"Health: {health.MaxHealth}");
            builder.AppendLine($"Armor Type: {health.Modifier.Name}");
        }

        Debug.Log(component);
        if (component is Turret turret)
        {
            builder.AppendLine($"Horizontal Speed {turret.HorizontalSpeed}");
            builder.AppendLine($"Vertical Speed {turret.VerticalSpeed}");
            builder.AppendLine($"Horizontal Range {turret.HorizontalRange}");
            builder.AppendLine($"Vertical Range {turret.VerticalRange}");
        }

        return builder.ToString();
    }
}
