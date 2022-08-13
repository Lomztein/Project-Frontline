using CustomGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UnitGroup : MonoBehaviour
{
    public Text Header;

    public RectTransform ButtonParent;
    public GameObject ButtonPrefab;
    public Toggle Toggle;

    private List<UnitToggle> _toggles = new List<UnitToggle>();

    public void SetEnabled(bool enabled)
    {
        Toggle.isOn = enabled;
    }

    public void OnToggleChanged(bool value)
    {
        foreach (var toggle in _toggles)
        {
            toggle.SetEnabled(value);
        }
    }
    
    public void Initialize(PlayerSettings settings, IGrouping<string, GameObject> group, Action<GameObject, bool> onUnitToggle)
    {
        foreach (var unit in group)
        {
            GameObject newButtonObj = Instantiate(ButtonPrefab, ButtonParent);
            UnitToggle toggle = newButtonObj.GetComponent<UnitToggle>();
            toggle.Apply(settings, unit, onUnitToggle);
            _toggles.Add(toggle);
        }
    }

    public void SetHeader (string value)
    {
        Header.text = value;
    }
}
