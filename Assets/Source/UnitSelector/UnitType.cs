using CustomGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UnitType : MonoBehaviour
{
    public Text Header;
    public Toggle Toggle;

    public RectTransform UnitGroupParent;
    public GameObject UnitGroupPrefab;

    private List<UnitGroup> _groups = new List<UnitGroup>();

    public void SetAll(bool value)
    {
        foreach (var group in _groups)
        {
            group.SetEnabled(value);
        }
    }

    public void Initialize(PlayerSettings settings, IEnumerable<IGrouping<string, GameObject>> unitsOfType, Action<GameObject, bool> onUnitToggle)
    {
        foreach (var group in unitsOfType)
        {
            GameObject newGroupObj = Instantiate(UnitGroupPrefab, UnitGroupParent);
            UnitGroup unitGroup = newGroupObj.GetComponent<UnitGroup>();
            unitGroup.Initialize(settings, group, onUnitToggle);
            _groups.Add(unitGroup);
            unitGroup.SetHeader(group.Key);
        }
    }

    public void SetHeader(string text)
    {
        Header.text = text;
    }
}
