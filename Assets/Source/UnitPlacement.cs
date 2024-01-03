using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using Util;

public class UnitPlacement : MonoBehaviour
{
    public bool Active => _prefab;

    public PlayerHandler Handler;
    public Color CanPlaceColor;
    public Color CannotAffordColor;
    public Color CannotPlaceColor;
    public LayerMask TerrainLayer;

    public Transform RangeIndicator;
    public float RangeIndicatorAlpha = .5f;

    public GameObject CurrentPlacement => _prefab;
    private GameObject _prefab;
    private GameObject _model;
    private Commander _commander;
    private Unit _unit;
    private OverlapUtils.OverlapShape _unitOverlapShape;

    private InputAction SelectAction;
    private InputAction CancelAction;
    private InputAction QuickPlaceAction;

    private void Start()
    {
        Handler.PlayerInput.onControlsChanged += OnUpdated;
        OnUpdated(Handler.PlayerInput);
    }

    private void OnUpdated(PlayerInput input)
    {
        SelectAction = input.actions["Select"];
        CancelAction = input.actions["Cancel"];
        QuickPlaceAction = input.actions["QuickPlace"];
    }

    public void TakeUnit (GameObject prefab, Commander commander)
    {
        Cancel();
        Reset();

        _unitOverlapShape = commander.GetUnitPlacementOverlapShape(prefab);
        GameObject placementPrefab = commander.GeneratePrefab(prefab);

        _model = UnityUtils.InstantiateMockGO(placementPrefab);

        _model.transform.SetParent(transform);
        _model.transform.position = transform.position;
        _model.transform.rotation = commander.Fortress.rotation;

        _prefab = prefab;
        _commander = commander;
        _unit = prefab.GetComponent<Unit>();

        AIController controller = placementPrefab.GetComponent<AIController>();
        float unitRange = 0f;
        if (controller)
        {
            unitRange = controller.AttackRange;
        }
        // Refactor to be more generic at some point.
        UnitFactorySpeedIncreaseStructure upgrader = placementPrefab.GetComponent<UnitFactorySpeedIncreaseStructure>();
        if (upgrader)
        {
            unitRange = upgrader.Range;
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
            Ray mouseRay = Handler.PointerToWorldRay();
            if (Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, TerrainLayer))
            {
                transform.position = hit.point;

                if (_commander.HasCredits(_unit.GetCost(_commander)))
                {
                    if (_commander.CanPlace(hit.point, _commander.transform.rotation, _unitOverlapShape))
                    {
                        SetModelColor(CanPlaceColor);
                        if (SelectAction.triggered && !UIHoverChecker.IsOverUI(Handler.GetPointerScreenPosition()))
                        {
                            _commander.TryPurchaseAndPlaceUnit(_prefab, hit.point, _commander.transform.rotation);

                            if (!(QuickPlaceAction.ReadValue<float>() > 0.5f))
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
            if (CancelAction.triggered)
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
}
