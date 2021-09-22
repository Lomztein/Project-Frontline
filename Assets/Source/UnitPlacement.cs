using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

public class UnitPlacement : MonoBehaviour
{
    public bool Active => _prefab;

    public Color CanPlaceColor;
    public Color CannotAffordColor;
    public Color CannotPlaceColor;
    public LayerMask TerrainLayer;

    public Transform RangeIndicator;
    public float RangeIndicatorAlpha = .5f;

    private GameObject _prefab;
    private GameObject _model;
    private Commander _commander;
    private Unit _unit;
    private Vector3 _placementCheckSize;

    public void TakeUnit (GameObject prefab, Commander commander)
    {
        Cancel();
        Reset();

        GameObject placementPrefab = commander.GeneratePrefab(prefab);

        _model = UnityUtils.InstantiateMockGO(placementPrefab);
        _model.transform.SetParent(transform);
        _model.transform.position = transform.position;

        _prefab = prefab;
        _commander = commander;
        _placementCheckSize = _commander.GetUnitPlacementCheckSize(_prefab);
        _unit = prefab.GetComponent<Unit>();

        AIController controller = placementPrefab.GetComponent<AIController>();
        float unitRange = 0f;
        if (controller)
        {
            unitRange = controller.AttackRange;
        }

        RangeIndicator.transform.localScale = new Vector3(unitRange, unitRange, unitRange);
    }

    private void SetModelColor (Color color)
    {
        foreach (var renderer in _model.GetComponentsInChildren<Renderer>())
        {
            renderer.material.color = color;
        }

        Color rangeIndicatorColor = new Color(color.r, color.g, color.b, RangeIndicatorAlpha);
        foreach (var renderer in RangeIndicator.GetComponentsInChildren<Renderer>())
        {
            renderer.material.color = rangeIndicatorColor;
        }
    }

    void Update()
    {
        if (Active)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, TerrainLayer))
            {
                transform.position = hit.point;

                if (_commander.HasCredits(_unit.Cost))
                {
                    if (CanPlace(hit.point, _placementCheckSize))
                    {
                        SetModelColor(CanPlaceColor);
                        if (Input.GetMouseButtonDown(0) && !UIHoverChecker.IsOverUI(Input.mousePosition))
                        {
                            _commander.TryPurchaseAndPlaceUnit(_prefab, hit.point, _commander.transform.rotation);

                            if (!Input.GetKey(KeyCode.LeftShift))
                            {
                                Cancel();
                            }
                        }
                    }
                    else
                    {
                        SetModelColor(CannotPlaceColor);
                    }
                }
                else
                {
                    SetModelColor(CannotAffordColor);
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cancel();
            }
        }
        else
        {
            Reset();
        }
    }

    public void Cancel ()
    {
        _prefab = null;
    }

    private void Reset()
    {
        RangeIndicator.transform.localScale = Vector3.zero;
        if (_model)
            Destroy(_model);
    }

    private bool CanPlace(Vector3 position, Vector3 checkSize)
    {
        Collider[] colliders = Physics.OverlapBox(position, checkSize / 2f, Quaternion.identity, ~TerrainLayer);
        return !colliders.Any(x => x.CompareTag("StructureUnit"));
    }
}
