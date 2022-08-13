using CustomGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSelector : MonoBehaviour
{
    public RectTransform TypesParent;
    public GameObject TypePrefab;
    private PlayerSettings _settings;

    public GameSettings MainWindow;

    public enum GroupingType { Prefix, Tier, Cost }
    public GroupingType GroupBy;

    private Dictionary<GameObject, bool> _unitToggle = new Dictionary<GameObject, bool>();

    public void SetEnabled(bool value)
    {
        gameObject.SetActive(value);
    }

    public void Initialize(PlayerSettings settings, IEnumerable<GameObject> units) 
    {
        Clear();

        _settings = settings;

        var unitArray = units.ToList();
        unitArray.Sort(new UnitComparer());

        _unitToggle = _settings.UnitAvailable;
        foreach (var unit in unitArray)
        {
            if (!_unitToggle.ContainsKey(unit))
            {
                _unitToggle.Add(unit, true);
            }
        }

        var typeGroup = unitArray.GroupBy(x => x.GetComponent<Unit>().Info.UnitType);
        foreach (var type in typeGroup)
        {
            var group = Group(GroupBy, type);
            GameObject unitTypeObj = Instantiate(TypePrefab, TypesParent);
            UnitType unitType = unitTypeObj.GetComponent<UnitType>();
            unitType.Initialize(_settings, group, OnUnitToggle);
            unitType.SetHeader(type.Key.ToString());
        }
    }

    public void Clear ()
    {
        foreach (Transform child in TypesParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void Cancel ()
    {
        SetEnabled(false);
        MainWindow.SetEnabled(true);
    }

    public void Accept ()
    {
        _settings.UnitAvailable = _unitToggle;
        SetEnabled(false);
        MainWindow.SetEnabled(true);
    }

    private void OnUnitToggle(GameObject unit, bool value)
    {
        _unitToggle[unit] = value;
    }

    private IEnumerable<IGrouping<string, GameObject>> Group (GroupingType groupType, IEnumerable<GameObject> gameObjects)
    {
        switch (groupType)
        {
            case GroupingType.Tier: return GroupByTier(gameObjects);
            case GroupingType.Prefix: return GroupByPrefix(gameObjects);
            case GroupingType.Cost: return GroupByCost(gameObjects);
        }
        throw new InvalidOperationException("Unsupported grouping.");
    }

    private IEnumerable<IGrouping<string, GameObject>> GroupByCost(IEnumerable<GameObject> gameObjects)
    {
        return gameObjects.GroupBy(x => GetCostGroup(x.GetComponent<Unit>(), 2000));
    }

    private string GetCostGroup (Unit unit, int groupRange)
    {
        int start = Mathf.FloorToInt(unit.Cost / (float)groupRange) * groupRange;
        int end = Mathf.CeilToInt(unit.Cost / (float)groupRange) * groupRange;
        return $"{start} - {end}";
    }

    private IEnumerable<IGrouping<string, GameObject>> GroupByPrefix(IEnumerable<GameObject> gameObjects)
    {
        return gameObjects.GroupBy(x => GetIdentifierPrefix(x.GetComponent<Unit>()));
    }

    private string GetIdentifierPrefix (Unit unit)
    {
        return unit.Info.Identifier.Substring(0, unit.Info.Identifier.IndexOf('.'));
    }

    private IEnumerable<IGrouping<string, GameObject>> GroupByTier(IEnumerable<GameObject> gameObjects)
    {
        return gameObjects.GroupBy(x => x.GetComponent<Unit>().Info.UnitTier.ToString()).ToList();
    }
}
