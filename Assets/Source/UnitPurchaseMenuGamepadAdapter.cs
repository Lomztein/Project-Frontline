using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitPurchaseMenuGamepadAdapter : MonoBehaviour
{
    public UnitPurchaseMenu Menu;
    public PlayerHandler Handler;
    public UnitPlacement Placement;

    public int TabIndex;
    public UnitTabButton[] Tabs;

    public int UnitIndex;
    public UnitButton[] UnitButtons;

    public InputAction SelectAction;
    public InputAction SwitchTabAction;
    public InputAction SwitchChangeSelectionAction;

    public RectTransform SelectionHighlighter;

    public void Assign(UnitPurchaseMenu menu, PlayerHandler handler, UnitPlacement placement)
    {
        Menu = menu;
        Handler = handler;
        Handler.PlayerInput.onControlsChanged += OnUpdated;
        Placement = placement;
        OnUpdated(Handler.PlayerInput);
    }

    private void Start()
    {
        StartCoroutine(DelayedInit());
        SelectionHighlighter = Instantiate(Resources.Load<GameObject>("Prefabs/UI/UnitButtonSelectionHighlighter"), Menu.transform).GetComponent<RectTransform>();
    }

    private IEnumerator DelayedInit()
    {
        yield return null;
        Tabs = Menu.GetComponentsInChildren<UnitTabButton>();
        SwitchTab(0);
    }

    private void OnUpdated(PlayerInput input)
    {
        SelectAction = input.actions["Select"];
        SwitchTabAction = input.actions["ChangeTab"];
        SwitchChangeSelectionAction = input.actions["ChangeSelection"];
    }

    private void Update()
    {
        if (SelectAction.triggered)
        {
            ForceClickUnitButton(UnitButtons[UnitIndex]);
        }

        if (SwitchTabAction.triggered)
        {
            int direction = Mathf.RoundToInt(SwitchTabAction.ReadValue<float>());
            SwitchTab(direction);
        }

        if (SwitchChangeSelectionAction.triggered)
        {
            int direction = Mathf.RoundToInt(SwitchChangeSelectionAction.ReadValue<float>());
            SwitchUnit(direction);
        }
    }

    public void SwitchTab(int direction)
    {
        int numTabs = Tabs.Length;
        TabIndex = (TabIndex + direction) % numTabs;
        if (TabIndex < 0) TabIndex = numTabs - 1;
        Menu.OpenTab(Tabs[TabIndex].Type);
        StartCoroutine(RefreshUnitButtons());
    }

    private IEnumerator RefreshUnitButtons()
    {
        yield return null;
        UnitButtons = Menu.GetComponentsInChildren<UnitButton>();
        SwitchUnit(0);
    }

    public void SwitchUnit(int direction)
    {
        int numUnits = UnitButtons.Length;
        UnitIndex = (UnitIndex + direction) % numUnits;
        if (UnitIndex < 0) UnitIndex = numUnits - 1;
        SelectionHighlighter.transform.position = UnitButtons[UnitIndex].transform.position;
        if (Placement.CurrentPlacement)
        {
            ForceClickUnitButton(UnitButtons[UnitIndex]);
        }
    }

    public void ForceClickUnitButton(UnitButton button)
    {
        button.Click();
    }
}
